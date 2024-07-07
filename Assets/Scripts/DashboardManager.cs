using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XCharts.Runtime;

public class DashboardManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject chartScrollView = null!;
    [SerializeField]
    private Transform chartContentTransform = null!;
    [SerializeField]
    private TMP_Text roomName = null!;
    [SerializeField]
    private TMP_Text roomCode = null!;
    [SerializeField]
    private Button pauseGameButton;
    [SerializeField]
    private Transform learnerNameList = null!;
    [SerializeField]
    private TextMeshProUGUI currentLearnerText = null!;
    
    [Header("Prefabs")]
    [SerializeField]
    private GameObject learnerInfoPrefab = null!;
    [SerializeField]
    private GameObject chartPanelPrefab = null!;

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

    private List<ChartPanel> _chartPanels = new();
    
    private void Awake()
    {
        var gameManager = GameManager.Singleton;
        if (gameManager.CurrentLobby != null)
        {
            roomName.text = gameManager.CurrentLobby.Name;
            roomCode.text = gameManager.CurrentLobby.LobbyCode;
        }

        if (gameManager.noDashboard)
        {
            chartScrollView.SetActive(false);
        }
        else
        {
            // init charts
            foreach (var chartType in Enum.GetValues(typeof(EChartType)).Cast<EChartType>())
            {
                var chartPanel = Instantiate(chartPanelPrefab, chartContentTransform)
                    .GetComponent<ChartPanel>();
                chartPanel.Init(chartType);
                _chartPanels.Add(chartPanel);
            }
        }
        
        pauseGameButton.onClick.AddListener(PauseGame);
        UpdateButtonText();
        UpdateLearnerNames();
        
        gameManager.PlayersLeftOrJoined += UpdateLearnerNames;
        gameManager.LearnerDataChanged += UpdateLearnerCharts;
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
    
    private void UpdateLearnerNames()
    {
        var gameManager = GameManager.Singleton;
        foreach (Transform child in learnerNameList.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (var learner in gameManager.Learners)
        {
            var learnerInfo = Instantiate(learnerInfoPrefab, learnerNameList);
            learner.LearnerInfoGameObject = learnerInfo;
            var learnerText = learnerInfo.GetComponentInChildren<TextMeshProUGUI>();
            learnerText.text = learner.Name;
            var learnerButton = learnerInfo.GetComponent<Button>();
            learnerButton.onClick.AddListener(() => OnLearnerClicked(learner));
        }

        // Mark the first item as clicked
        if (gameManager.Learners.Count > 0)
        {
            OnLearnerClicked(gameManager.Learners.First());
        }
    }

    private void OnLearnerClicked(LearnPlayer learner)
    {
        var gameManager = GameManager.Singleton;
        // Reset all items to default state
        foreach (var learnerGo in gameManager.Learners.Select(x => x.LearnerInfoGameObject))
        {
            learnerGo.GetComponent<Image>().color = gameManager.gameSettings.theme.backgroundColor;
        }

        if (gameManager.noDashboard)
        {
            return;
        }

        // Mark the clicked item
        learner.LearnerInfoGameObject.GetComponent<Image>().color = gameManager.gameSettings.theme.colorPalette[0];
        gameManager.CurrentLearner = learner;
        currentLearnerText.text = $"Current Learner: {learner.Name}";
        gameManager.FetchLearnerAnalytics();
        UpdateSummary();
    }

    #endregion

    #region Charts

    private void UpdateLearnerCharts()
    {
        var gameManager = GameManager.Singleton;

        // Update overall performance
        var performance = overallPerformance.series.First();
        performance.data[0].data = new List<double>
            { Math.Round(gameManager.CurrentLearner.ScalarBeliefsList.Last().MasteryOfSortingAlgorithm.Value, 3), 1 };
        performance.data[1].data = new List<double> 
            { Math.Round(gameManager.CurrentLearner.ScalarBeliefsList.Last().LearnBasicSkills.Value, 3), 1 };
        performance.data[2].data = new List<double>
            { Math.Round(gameManager.CurrentLearner.ScalarBeliefsList.Last().LearnBehaviourOfSortingAlgorithms.Value, 3), 1 };
        overallPerformance.RefreshChart(performance);

        foreach (var chartPanel in _chartPanels)
        {
            chartPanel.UpdateCharts();
        }
        
        // // Update Observation Count
        // bubbleSortChartPanel.subSkill1ObservationCountText.text = gameManager.CurrentLearner.ObservationCount.ToString();
        // bubbleSortChartPanel.subSkill2ObservationCountText.text = gameManager.CurrentLearner.ObservationCount.ToString(); // TODO: this is wrong
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
