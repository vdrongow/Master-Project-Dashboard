using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public DashboardConfig Config = null!;
    
    [Header("UI Elements for Starting a Lobby")]
    [SerializeField]
    private GameObject startSessionPanel = null!;
    [SerializeField]
    private TMP_InputField lobbyNameInputField = null!;
    [SerializeField]
    private Button createSessionBtn = null!;
    
    [Header("UI Elements for Lobby")]
    [SerializeField]
    private GameObject lobbyPanel = null!;
    [SerializeField]
    private TMP_Text roomName = null!;
    [SerializeField]
    private TMP_Text roomCode = null!;
    [SerializeField]
    private TMP_Text availableSlotsText = null!;
    [SerializeField]
    private GameObject playerListContent = null!;
    [SerializeField]
    private GameObject playerTextPrefab = null!;
    [SerializeField]
    private Button startGameBtn = null!;
    [SerializeField]
    private Button deleteLobbyBtn = null!;
    
    private float _roomUpdateTime;

    private void Awake()
    {
        startSessionPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        
        if (PlayerPrefs.HasKey("LobbyName"))
        {
            lobbyNameInputField.text = PlayerPrefs.GetString("LobbyName");
        }
        
        createSessionBtn.onClick.AddListener(CreateLobby);
    }
    
    private void Update()
    {
        HandleRoomUpdate();
    }
    
    private async void CreateLobby()
    {
        var gameManager = GameManager.Singleton;
        try
        {
            var lobbyName = lobbyNameInputField.text;
            if(string.IsNullOrEmpty(lobbyName))
            {
                lobbyName = Config.defaultLobbyName;
            }
            PlayerPrefs.SetString("LobbyName", lobbyName);
            var maxPlayers = Config.maxLobbySize;
            var options = new CreateLobbyOptions
            {
                IsPrivate = Config.isPrivateLobby,
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {
                            Constants.PLAYER_NAME,
                            new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, gameManager.playerId)
                        }
                    }
                },
                Data = new Dictionary<string, DataObject>
                {
                    {
                        Constants.LOBBY_IS_GAME_STARTED,
                        new DataObject(DataObject.VisibilityOptions.Member, false.ToString(), DataObject.IndexOptions.S1)
                    },
                    {
                        Constants.LOBBY_IS_GAME_PAUSED,
                        new DataObject(DataObject.VisibilityOptions.Member, false.ToString(), DataObject.IndexOptions.S2)
                    }
                }
            };
            gameManager.CurrentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            startSessionPanel.SetActive(false);
            lobbyPanel.SetActive(true);
            roomName.text = gameManager.CurrentLobby.Name;
            roomCode.text = gameManager.CurrentLobby.LobbyCode;
            startGameBtn.onClick.AddListener(StartGame);
            deleteLobbyBtn.onClick.AddListener(DeleteLobby);
            HandleRoomUpdate();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    
    private async void HandleRoomUpdate()
    {
        var gameManager = GameManager.Singleton;
        if (gameManager.CurrentLobby == null || gameManager.IsGameStarted())
        {
            return;
        }
        _roomUpdateTime -= Time.deltaTime;
        if (_roomUpdateTime <= 0)
        {
            _roomUpdateTime = 2f;
            try
            {
                if (gameManager.IsinLobby())
                {
                    gameManager.CurrentLobby = await LobbyService.Instance.GetLobbyAsync(gameManager.CurrentLobby.Id);
                    // delete all children of playerList
                    foreach (Transform child in playerListContent.transform)
                    {
                        Destroy(child.gameObject);
                    }
                    foreach (var player in gameManager.CurrentLobby.Players)
                    {
                        if(player.Id == gameManager.playerId)
                        {
                            var playerText = Instantiate(playerTextPrefab, playerListContent.transform);
                            playerText.GetComponentInChildren<TMP_Text>().text = "(You)";
                            playerText.GetComponentInChildren<TMP_Text>().color = Color.grey;
                        }
                        else
                        {
                            var playerName = player.Data[Constants.PLAYER_NAME].Value;
                            var playerText = Instantiate(playerTextPrefab, playerListContent.transform);
                            playerText.GetComponentInChildren<TMP_Text>().text = playerName;
                        }
                    }
                    
                    availableSlotsText.text = gameManager.CurrentLobby.MaxPlayers - gameManager.CurrentLobby.AvailableSlots + "/" +
                                              gameManager.CurrentLobby.MaxPlayers;
                }
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                if (e.Reason is LobbyExceptionReason.Forbidden or LobbyExceptionReason.LobbyNotFound)
                {
                    gameManager.CurrentLobby = null;
                }
            }
        }
    }
    
    private async void StartGame()
    {
        var gameManager = GameManager.Singleton;
        if (gameManager.CurrentLobby == null || !gameManager.IsHost())
        {
            return;
        }
        try
        {
            gameManager.SetGameStarted(true);
            GameManager.Singleton.playerList = gameManager.CurrentLobby.Players.Select(p => p.Data[Constants.PLAYER_NAME].Value).ToList();
            SceneManager.LoadScene(Config.dashboardScene);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    
    private void DeleteLobby()
    {
        var gameManager = GameManager.Singleton;
        if (gameManager.CurrentLobby == null)
        {
            return;
        }
        LobbyService.Instance.DeleteLobbyAsync(gameManager.CurrentLobby.Id);
        gameManager.CurrentLobby = null;
        lobbyPanel.SetActive(false);
        startSessionPanel.SetActive(true);
    }
}
