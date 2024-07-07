using System;

namespace Enums
{
    [Serializable]
    public enum EChartType
    {
        BubbleSort = 0,
        SelectionSort = 1,
        InsertionSort = 2,
        ExtremeValues = 3,
        NumberComparison = 4,
    }

    public static class EChartTypeExtensions
    {
        public static string AsString(this EChartType chartType) => chartType switch
        {
            EChartType.BubbleSort => nameof(EChartType.BubbleSort),
            EChartType.SelectionSort => nameof(EChartType.SelectionSort),
            EChartType.InsertionSort => nameof(EChartType.InsertionSort),
            EChartType.ExtremeValues => nameof(EChartType.ExtremeValues),
            EChartType.NumberComparison => nameof(EChartType.NumberComparison),
            _ => throw new ArgumentOutOfRangeException(nameof(chartType), chartType, null)
        };
    }
}