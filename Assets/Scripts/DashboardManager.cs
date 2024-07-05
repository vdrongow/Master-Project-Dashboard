using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XCharts.Runtime;

public class DashboardManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private TMP_Text roomName = null!;
    [SerializeField]
    private TMP_Text roomCode = null!;
    [SerializeField]
    private Button pauseGameButton;

    [Header("Charts")]
    [SerializeField] 
    private RingChart overallPerformance = null!;

    [Space]
    [SerializeField]
    private TMP_Text levelsCompletedText = null!;
    [SerializeField] 
    private TMP_Text timePlayedText = null!;
    [SerializeField] 
    private TMP_Text mistakesMadeText = null!;
    
    private void Awake()
    {
        var gameManager = GameManager.Singleton;
        if (gameManager.CurrentLobby != null)
        {
            roomName.text = gameManager.CurrentLobby.Name;
            roomCode.text = gameManager.CurrentLobby.LobbyCode;
        }
        
        pauseGameButton.onClick.AddListener(PauseGame);
        UpdateButtonText();
        gameManager.LearnerDataChanged += UpdateOverallPerformance;
        gameManager.PlayerDataChanged += UpdateSummary;
    }

    #region Game Control

    private void PauseGame()
    {
        var gameManager = GameManager.Singleton;
        gameManager.SetGamePaused(!gameManager.isGamePaused);
        UpdateButtonText();
    }
    
    private void UpdateButtonText()
    {
        var gameManager = GameManager.Singleton;
        pauseGameButton.GetComponentInChildren<TextMeshProUGUI>().text = gameManager.isGamePaused ? "Resume Game" : "Pause Game";
    }

    #endregion

    #region Charts

    private void UpdateOverallPerformance()
    {
        var gameManager = GameManager.Singleton;

        var series = overallPerformance.series.First();
        series.data[0].data = new List<double>
            { Math.Round(gameManager.CurrentLearner.MasteryOfSortingAlgorithm, 3), 1 };
        series.data[1].data = new List<double> 
            { Math.Round(gameManager.CurrentLearner.LearnBasicSkills, 3), 1 };
        series.data[2].data = new List<double>
            { Math.Round(gameManager.CurrentLearner.LearnBehaviourOfSortingAlgorithm, 3), 1 };
    }

    private void UpdateSummary()
    {
        var gameManager = GameManager.Singleton;
        levelsCompletedText.text = gameManager.CurrentLearner.FinishedLevels.ToString();
        // show the played time in minutes and seconds
        var time = gameManager.CurrentLearner.TotalPlayedTime;
        var minutes = time / 60;
        var seconds = time % 60;
        timePlayedText.text = $"{minutes:00}:{seconds:00}";
        mistakesMadeText.text = gameManager.CurrentLearner.TotalMistakes.ToString();
    }

    #endregion
}
