using System.Collections.Generic;
using System.Linq;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public sealed class GameManager : MonoBehaviour
{
    public static GameManager Singleton { get; private set; } = null!;
    
    public Lobby CurrentLobby;
    public string playerId;
    public float heartbeatTime = 15f;
    
    [HideInInspector]
    public List<string> playerList = new List<string>();
    
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
    }
    
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            playerId = AuthenticationService.Instance.PlayerId;
            Debug.Log("Signed in " + playerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
    
    private void Update()
    {
        HandleLobbyHeartbeat();
    }
    
    private async void HandleLobbyHeartbeat()
    {
        if (CurrentLobby == null || !IsHost())
        {
            return;
        }
        heartbeatTime -= Time.deltaTime;
        if (heartbeatTime <= 0)
        {
            heartbeatTime = 15f;
            await LobbyService.Instance.SendHeartbeatPingAsync(CurrentLobby.Id);
        }
    }
    
    public async void SetGameStarted(bool isStarted)
    {
        if (CurrentLobby == null || !IsHost())
        {
            return;
        }
        try
        {
            var updateOptions = new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { Constants.LOBBY_IS_GAME_STARTED, new DataObject(DataObject.VisibilityOptions.Member, isStarted.ToString()) },
                    { Constants.LOBBY_IS_GAME_PAUSED, new DataObject(DataObject.VisibilityOptions.Member, IsGamePaused().ToString()) }
                }
            };

            CurrentLobby = await LobbyService.Instance.UpdateLobbyAsync(CurrentLobby.Id, updateOptions);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    
    public async void SetPauseGame(bool isPaused)
    {
        if (CurrentLobby == null || !IsHost())
        {
            return;
        }
        try
        {
            var updateOptions = new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { Constants.LOBBY_IS_GAME_STARTED, new DataObject(DataObject.VisibilityOptions.Member, IsGameStarted().ToString()) },
                    { Constants.LOBBY_IS_GAME_PAUSED, new DataObject(DataObject.VisibilityOptions.Member, isPaused.ToString()) }
                }
            };

            CurrentLobby = await LobbyService.Instance.UpdateLobbyAsync(CurrentLobby.Id, updateOptions);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public bool IsHost()
    {
        if (CurrentLobby == null)
        {
            return false;
        }
        return CurrentLobby.HostId == playerId;
    }
    
    public bool IsinLobby()
    {
        if (CurrentLobby.Players.Any(player => player.Id == playerId))
        {
            return true;
        }

        CurrentLobby = null;
        return false;
    }
    
    public bool IsGameStarted()
    {
        if (CurrentLobby == null)
        {
            return false;
        }
        return CurrentLobby.Data[Constants.LOBBY_IS_GAME_STARTED].Value == true.ToString();
    }
    
    public bool IsGamePaused()
    {
        if(CurrentLobby == null)
        {
            return false;
        }
        return CurrentLobby.Data[Constants.LOBBY_IS_GAME_PAUSED].Value == true.ToString();
    }
}