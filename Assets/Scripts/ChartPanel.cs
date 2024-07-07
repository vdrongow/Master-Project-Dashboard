using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using TMPro;
using UnityEngine;
using XCharts.Runtime;

public class ChartPanel : MonoBehaviour
{
    public EChartType chartType;
    
    [Header("References")]
    [SerializeField]
    private TextMeshProUGUI skillText;
    [SerializeField]
    private TextMeshProUGUI subSkill1Text;
    [SerializeField]
    private TextMeshProUGUI subSkill2Text;
    
    [Space]
    public RingChart skillRingChart;
    public RingChart subSkill1RingChart;
    public RingChart subSkill2RingChart;
    
    [Space]
    public LineChart performanceLineChart;

    [Space]
    public BarChart subSkill1ObservationBarChart;
    public BarChart subSkill2ObservationBarChart;

    public void Init(EChartType type)
    {
        chartType = type;
        
        skillText.text = chartType.AsString();
        var subSkillNames = GetSubSkillNames();
        subSkill1Text.text = subSkillNames.subSkill1;
        subSkill2Text.text = subSkillNames.subSkill2;

        skillRingChart.GetChartComponent<Title>().text = chartType.AsString();
        subSkill1RingChart.GetChartComponent<Title>().text = subSkillNames.subSkill1;
        subSkill2RingChart.GetChartComponent<Title>().text = subSkillNames.subSkill2;
    }

    public void UpdateCharts()
    {
        UpdateCurrentPerformance();
        UpdatePerformanceOverTime();
        UpdateObservations();
    }

    private void UpdateCurrentPerformance()
    {
        var gameManager = GameManager.Singleton;
        var scalarBeliefValues = GetScalarBeliefValues(gameManager.CurrentLearner.ScalarBeliefsList.Last());

        var skillPerformance = skillRingChart.series.First();
        skillPerformance.data[0].data = new List<double> { Math.Round(scalarBeliefValues.skill, 3), 1 };
        skillRingChart.RefreshChart(skillPerformance);

        var subSkill1Performance = subSkill1RingChart.series.First();
        subSkill1Performance.data[0].data = new List<double> { Math.Round(scalarBeliefValues.subSkill1, 3), 1 };
        subSkill1RingChart.RefreshChart(subSkill1Performance);

        var subSkill2Performance = subSkill2RingChart.series.First();
        subSkill2Performance.data[0].data = new List<double> { Math.Round(scalarBeliefValues.subSkill2, 3), 1 };
        subSkill2RingChart.RefreshChart(subSkill2Performance);
    }

    private void UpdatePerformanceOverTime()
    {
        var gameManager = GameManager.Singleton;
        var trendSeries = performanceLineChart.series.First();
        trendSeries.data.Clear();

        var scalarBeliefsCount = gameManager.CurrentLearner.ScalarBeliefsList.Count;

        for (var i = 0; i < scalarBeliefsCount; i++)
        {
            var scalarBeliefValue = GetSkillScalarBeliefValue(gameManager.CurrentLearner.ScalarBeliefsList[i]);
            var value = Math.Round(scalarBeliefValue, 3);
            trendSeries.data.Add(new SerieData { data = new List<double> { i, value } });
        }

        performanceLineChart.RefreshChart(trendSeries);
    }

    private void UpdateObservations()
    {
        var gameManager = GameManager.Singleton;
        var probabilisticBeliefs =
            GetProbabilisticBeliefValues(gameManager.CurrentLearner.ProbabilisticBeliefsList.Last());

        var subSkill1ObservationsSeries0 = subSkill1ObservationBarChart.series[0];
        subSkill1ObservationsSeries0.data[0].data = new List<double>
            { 0, Math.Round(probabilisticBeliefs.subSkill1.Good, 3) };
        subSkill1ObservationBarChart.RefreshChart(subSkill1ObservationsSeries0);

        var subSkill1ObservationsSeries1 = subSkill1ObservationBarChart.series[1];
        subSkill1ObservationsSeries1.data[0].data = new List<double>
            { 0, Math.Round(probabilisticBeliefs.subSkill1.Bad, 3) };
        subSkill1ObservationBarChart.RefreshChart(subSkill1ObservationsSeries1);

        var subSkill2ObservationsSeries0 = subSkill2ObservationBarChart.series[0];
        subSkill2ObservationsSeries0.data[0].data = new List<double>
            { 0, Math.Round(probabilisticBeliefs.subSkill2.Good, 3) };
        subSkill2ObservationBarChart.RefreshChart(subSkill2ObservationsSeries0);

        var subSkill2ObservationsSeries1 = subSkill2ObservationBarChart.series[1];
        subSkill2ObservationsSeries1.data[0].data = new List<double>
            { 0, Math.Round(probabilisticBeliefs.subSkill2.Bad, 3) };
        subSkill2ObservationBarChart.RefreshChart(subSkill2ObservationsSeries1);
    }
    
    private double GetSkillScalarBeliefValue(ScalarBeliefs scalarBeliefs)
    {
        return chartType switch
        {
            EChartType.BubbleSort => scalarBeliefs.BubbleSort.Value,
            EChartType.SelectionSort => scalarBeliefs.SelectionSort.Value,
            EChartType.InsertionSort => scalarBeliefs.InsertionSort.Value,
            EChartType.ExtremeValues => scalarBeliefs.ExtremeValues.Value,
            EChartType.NumberComparison => scalarBeliefs.NumberComparison.Value,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    private (ProbabilisticBelief subSkill1, ProbabilisticBelief subSkill2) GetProbabilisticBeliefValues(ProbabilisticBeliefs probabilisticBeliefs)
    {
        return chartType switch
        {
            EChartType.BubbleSort => (probabilisticBeliefs.SwapElements, probabilisticBeliefs.StepOver),
            EChartType.SelectionSort => (probabilisticBeliefs.FoundNewMin, probabilisticBeliefs.NoNewMin),
            EChartType.InsertionSort => (probabilisticBeliefs.SwapFurtherForwards, probabilisticBeliefs.InsertElement),
            EChartType.ExtremeValues => (probabilisticBeliefs.IdentifySmallestElement, probabilisticBeliefs.IdentifyLargestElement),
            EChartType.NumberComparison => (probabilisticBeliefs.IdentifySmallerNumber, probabilisticBeliefs.IdentifyLargerNumber),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    private (double skill, double subSkill1, double subSkill2) GetScalarBeliefValues(ScalarBeliefs scalarBeliefs)
    {
        return chartType switch
        {
            EChartType.BubbleSort => (scalarBeliefs.BubbleSort.Value, scalarBeliefs.SwapElements.Value, scalarBeliefs.StepOver.Value),
            EChartType.SelectionSort => (scalarBeliefs.SelectionSort.Value, scalarBeliefs.FoundNewMin.Value, scalarBeliefs.NoNewMin.Value),
            EChartType.InsertionSort => (scalarBeliefs.InsertionSort.Value, scalarBeliefs.SwapFurtherForwards.Value, scalarBeliefs.InsertElement.Value),
            EChartType.ExtremeValues => (scalarBeliefs.ExtremeValues.Value, scalarBeliefs.IdentifySmallestElement.Value, scalarBeliefs.IdentifyLargestElement.Value),
            EChartType.NumberComparison => (scalarBeliefs.NumberComparison.Value, scalarBeliefs.IdentifySmallerNumber.Value, scalarBeliefs.IdentifyLargerNumber.Value),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private (string subSkill1, string subSkill2) GetSubSkillNames()
    {
        return chartType switch
        {
            EChartType.BubbleSort => ("Swap Elements", "Step Over"),
            EChartType.SelectionSort => ("Found New Min", "No New Min"),
            EChartType.InsertionSort => ("Swap Further Forwards", "Insert Element"),
            EChartType.ExtremeValues => ("Identify Smallest Element","Identify Largest Element"),
            EChartType.NumberComparison => ("Identify Smaller Number", "Identify Larger Number"),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}