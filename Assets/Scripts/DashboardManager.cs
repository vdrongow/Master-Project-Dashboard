using System;
using System.Collections.Generic;
using System.Linq;
using Adlete;
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
        UpdateOverallPerformance();
    }

    private void Start()
    {
        var moduleConnection = ModuleConnection.Singleton;
        // check the status of the module connection
        moduleConnection.CheckStatus(
            info => Debug.Log($"StatusCode: {info.statusCode}, StatusMessage: {info.statusDescription}, TimeStamp: {info.timestamp}"),
            errorString => Debug.Log($"Error while checking status: {errorString}"),
            () => Debug.Log("CheckStatus finished"));
        
        // fetch service configuration
        moduleConnection.FetchServiceConfiguration(
            config => Debug.Log($"ServiceConfiguration: {string.Join(",", config.activityNames)}, {string.Join(",", config.initialScalarBeliefIds)}"),
            errorString => Debug.Log($"Error while fetching service configuration: {errorString}"),
            () => Debug.Log("FetchServiceConfiguration finished"));
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
            { Math.Round(gameManager.CurrentLearner.MasteryOfSortingAlgorithm, 2), 1 };
        series.data[1].data = new List<double> 
            { Math.Round(gameManager.CurrentLearner.LearnBasicSkills, 2), 1 };
        series.data[2].data = new List<double>
            { Math.Round(gameManager.CurrentLearner.LearnBehaviourOfSortingAlgorithm, 2), 1 };
    }

    #endregion
}
