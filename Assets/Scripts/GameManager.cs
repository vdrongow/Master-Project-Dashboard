﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Adlete;
using Newtonsoft.Json;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public sealed class GameManager : MonoBehaviour
{
    public static GameManager Singleton { get; private set; } = null!;
    
    public static Config Config { get; private set; } = null!;
    
    [Header("Configs")]
    public GameSettings gameSettings = null!;

    public bool noDashboard = false;
    
    [Header("Network Lobby")]
    public Lobby CurrentLobby;
    public string playerId;
    public bool isGamePaused;
    public float heartbeatTime = 15f;
    
    public List<LearnPlayer> Learners = new();
    public LearnPlayer CurrentLearner;
    
    public Action LearnerDataChanged;
    public Action PlayerDataChanged;
    public Action PlayersLeftOrJoined;
    
    private float _time;
    
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
        
        // Load the config file
        var configJson = File.ReadAllText(Constants.PATH_TO_CONFIG);
        Config = JsonConvert.DeserializeObject<Config>(configJson);
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

        if (IsGameStarted() && !noDashboard)
        {
            _time += Time.deltaTime;
            if (_time > gameSettings.adlete_requestInterval)
            {
                _time = 0.0f;
                FetchLearnerAnalytics();
            }
        }
    }

    #region Game Control

    public void StartGame()
    {
        SetGameStarted(true);
        if (Learners.Count > 0)
        {
            CurrentLearner = Learners.First();
        }
        // set time to Interval for fetching data at start
        _time = gameSettings.adlete_requestInterval;
    }

    #endregion

    #region Network Lobby
    
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

    private LearnPlayer CreateLearnPlayer(int index, Player lobbyPlayer)
    {
        var learner = new LearnPlayer(
            name: lobbyPlayer.Data[Constants.PLAYER_NAME].Value,
            playerId: lobbyPlayer.Id,
            lobbyId: index,
            playerDataChanged: () => PlayerDataChanged.Invoke()
        );
        Learners.Add(learner);
        return learner;
    }
    
    public async void SubscribeToLobbyEvents()
    {
        var callbacks = new LobbyEventCallbacks();
        callbacks.PlayerDataChanged += OnPlayerDataChanged;
        callbacks.PlayerJoined += OnPlayerJoined;
        callbacks.PlayerLeft += OnPlayerLeft;
        try
        {
            await Lobbies.Instance.SubscribeToLobbyEventsAsync(CurrentLobby.Id, callbacks);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private void OnPlayerJoined(List<LobbyPlayerJoined> lobbyPlayersJoined)
    {
        foreach (var lobbyPlayer in lobbyPlayersJoined)
        {
            if (lobbyPlayer.Player.Id == playerId)
            {
                continue;
            }
            var learner = CreateLearnPlayer(lobbyPlayer.PlayerIndex, lobbyPlayer.Player);
            CurrentLearner ??= learner;
        }
        PlayersLeftOrJoined?.Invoke();
    }

    private void OnPlayerLeft(List<int> lobbyIds)
    {
        foreach (var lobbyId in lobbyIds)
        {
            var learner = Learners.Find(l => l.LobbyId == lobbyId);
            Learners.Remove(learner);
        }
        PlayersLeftOrJoined?.Invoke();
    }
        
    private void OnPlayerDataChanged(Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> changedData)
    {
        if (noDashboard)
        {
            return;
        }
        foreach (var (lobbyId, changedOrRemovedLobbyValues) in changedData)
        {
            foreach (var (key, value) in changedOrRemovedLobbyValues)
            {
                var learner = Learners.Find(learner => learner.LobbyId == lobbyId);
                if (learner == null)
                {
                    continue;
                }
                learner.UpdatePlayerData(key, value.Value);
                Debug.Log($"Player {learner.Name} with ID {lobbyId} changed {key} to {value.Value.Value}");
            }
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
                    { Constants.LOBBY_IS_GAME_STARTED, new DataObject(DataObject.VisibilityOptions.Member, isStarted.ToString()) }
                }
            };
            CurrentLobby = await LobbyService.Instance.UpdateLobbyAsync(CurrentLobby.Id, updateOptions);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    
    public async void SetGamePaused(bool isPaused)
    {
        if (CurrentLobby == null || !IsHost())
        {
            return;
        }
        try
        {
            isGamePaused = isPaused;
            var updateOptions = new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
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
    
    #endregion

    #region Adlete

    public void FetchLearnerAnalytics()
    {
        if (CurrentLearner == null)
        {
            return;
        }
        var moduleConnection = ModuleConnection.Singleton;
        var adleteLearnerId = moduleConnection.GetLearnerIDFromUsername(CurrentLearner.Name);
        
        moduleConnection.LearnerAnalytics(adleteLearnerId, data =>
        {
            // TODO: Only add new elements, do not always clear list
            CurrentLearner.ScalarBeliefsList.Clear();
            foreach (var jsonString in data.learner.scalarBeliefs)
            {
                var scalarBeliefs = JsonConvert.DeserializeObject<ScalarBeliefs>(jsonString);
                CurrentLearner.ScalarBeliefsList.Add(scalarBeliefs);
            }
             
            CurrentLearner.ProbabilisticBeliefsList.Clear();
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new ProbabilisticValueConverter());
            foreach (var jsonString in data.learner.probabilisticBeliefs)
            {
                var probabilisticBeliefs = JsonConvert.DeserializeObject<ProbabilisticBeliefs>(jsonString, settings);
                CurrentLearner.ProbabilisticBeliefsList.Add(probabilisticBeliefs);
            }
            LearnerDataChanged.Invoke();
        });
    }

    #endregion
}