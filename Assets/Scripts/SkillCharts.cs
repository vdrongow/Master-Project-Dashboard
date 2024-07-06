using TMPro;
using UnityEngine;
using XCharts.Runtime;

public class SkillCharts : MonoBehaviour
{
    [Header("References")]
    public RingChart skillRingChart;
    public RingChart subSkill1RingChart;
    public RingChart subSkill2RingChart;
    
    [Space]
    public LineChart performanceLineChart;

    [Space]
    public TextMeshProUGUI subSkill1ObservationCountText;
    public BarChart subSkill1ObservationBarChart;
    public TextMeshProUGUI subSkill2ObservationCountText;
    public BarChart subSkill2ObservationBarChart;
}