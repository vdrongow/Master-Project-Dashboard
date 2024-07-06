using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LearnPlayer
{
    public string Name { get; set; }
    public GameObject LearnerInfoGameObject;
    public string PlayerId { get; set; }
    public int LobbyId { get; set; }
    public int FinishedLevels { get; set; }
    public int TotalMistakes { get; set; }
    public int TotalPlayedTime { get; set; }

    public int ObservationCount;
    public List<ScalarBeliefs> ScalarBeliefsList = new();
    public List<ProbabilisticBeliefs> ProbabilisticBeliefsList = new();

    private Action _playerDataChanged;

    public LearnPlayer(
        string name, 
        string playerId,
        int lobbyId, 
        int finishedLevels = 0, 
        int totalMistakes = 0,
        int totalPlayedTime = 0,
        Action playerDataChanged = null)
    {
        Name = name;
        PlayerId = playerId;
        LobbyId = lobbyId;
        FinishedLevels = finishedLevels;
        TotalMistakes = totalMistakes;
        TotalPlayedTime = totalPlayedTime;
        _playerDataChanged = playerDataChanged;
    }

    public void UpdatePlayerData(string key, PlayerDataObject valueValue)
    {
        switch (key)
        {
            case Constants.PLAYER_FINISHED_LEVELS:
                FinishedLevels = int.Parse(valueValue.Value);
                break;
            case Constants.PLAYER_TOTAL_MISTAKES:
                TotalMistakes = int.Parse(valueValue.Value);
                break;
            case Constants.PLAYER_TOTAL_PLAYED_TIME:
                TotalPlayedTime = int.Parse(valueValue.Value);
                break;
        }
        _playerDataChanged?.Invoke();
    }
}