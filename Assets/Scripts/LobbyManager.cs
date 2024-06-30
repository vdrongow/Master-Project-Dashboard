using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Singleton { get; private set; } = null!;
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
    private GameObject playerList = null!;
    [SerializeField]
    private Button startGameBtn = null!;
    [SerializeField]
    private Button deleteLobbyBtn = null!;
    
    private Lobby _currentLobby;
    private string _playerId;
    private float _heartbeatTime = 15f;
    private float _roomUpdateTime;

    private void Awake()
    {
        if(Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(this);
        }
        else if(Singleton != this)
        {
            Destroy(gameObject);
        }
        
        startSessionPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        
        if (PlayerPrefs.HasKey("LobbyName"))
        {
            lobbyNameInputField.text = PlayerPrefs.GetString("LobbyName");
        }
    }
    
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            _playerId = AuthenticationService.Instance.PlayerId;
            Debug.Log("Signed in " + _playerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        createSessionBtn.onClick.AddListener(CreateLobby);
    }
    
    private void Update()
    {
        HandleLobbyHeartbeat();
        HandleRoomUpdate();
    }
    
    private async void CreateLobby()
    {
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
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    { "IsGameStarted", new DataObject(DataObject.VisibilityOptions.Member, "false") }
                }
            };
            _currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            startSessionPanel.SetActive(false);
            lobbyPanel.SetActive(true);
            roomName.text = _currentLobby.Name;
            roomCode.text = _currentLobby.LobbyCode;
            startGameBtn.onClick.AddListener(StartGame);
            deleteLobbyBtn.onClick.AddListener(DeleteLobby);
            HandleRoomUpdate();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    
    private Player GetPlayer()
    {
        var player = new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, _playerId) }
            }
        };

        return player;
    }
    
    private async void HandleLobbyHeartbeat()
    {
        if (_currentLobby == null || !IsHost())
        {
            return;
        }
        _heartbeatTime -= Time.deltaTime;
        if (_heartbeatTime <= 0)
        {
            _heartbeatTime = 15f;
            await LobbyService.Instance.SendHeartbeatPingAsync(_currentLobby.Id);
        }
    }
    
    private async void HandleRoomUpdate()
    {
        if (_currentLobby == null || IsGameStarted())
        {
            return;
        }
        _roomUpdateTime -= Time.deltaTime;
        if (_roomUpdateTime <= 0)
        {
            _roomUpdateTime = 2f;
            try
            {
                if (IsinLobby())
                {
                    _currentLobby = await LobbyService.Instance.GetLobbyAsync(_currentLobby.Id);
                    var playerNames = _currentLobby.Players.Select(p => p.Data["PlayerName"].Value);
                    // delete all children of playerList
                    foreach (Transform child in playerList.transform)
                    {
                        Destroy(child.gameObject);
                    }
                    foreach (var playerName in playerNames)
                    {
                        var playerText = new GameObject("PlayerText", typeof(RectTransform));
                        playerText.transform.SetParent(playerList.transform);
                        playerText.AddComponent<TextMeshProUGUI>().text = playerName;
                    }
                    
                    availableSlotsText.text = _currentLobby.MaxPlayers - _currentLobby.AvailableSlots + "/" +
                        _currentLobby.MaxPlayers;
                }
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                if (e.Reason is LobbyExceptionReason.Forbidden or LobbyExceptionReason.LobbyNotFound)
                {
                    _currentLobby = null;
                }
            }
        }
    }
    
    private async void StartGame()
    {
        if (_currentLobby == null || !IsHost())
        {
            return;
        }
        try
        {
            var updateOptions = new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { "IsGameStarted", new DataObject(DataObject.VisibilityOptions.Member, "true") }
                }
            };

            _currentLobby = await LobbyService.Instance.UpdateLobbyAsync(_currentLobby.Id, updateOptions);
            SceneManager.LoadScene(Config.dashboardScene);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    
    private void DeleteLobby()
    {
        if (_currentLobby == null)
        {
            return;
        }
        LobbyService.Instance.DeleteLobbyAsync(_currentLobby.Id);
        _currentLobby = null;
        lobbyPanel.SetActive(false);
        startSessionPanel.SetActive(true);
    }

    private bool IsHost() => _currentLobby != null && _currentLobby.HostId == _playerId;
    
    private bool IsinLobby()
    {
        if (_currentLobby.Players.Any(player => player.Id == _playerId))
        {
            return true;
        }

        _currentLobby = null;
        return false;
    }
    
    private bool IsGameStarted()
    {
        if (_currentLobby == null)
        {
            return false;
        }
        return _currentLobby.Data["IsGameStarted"].Value == "true";
    }
}
