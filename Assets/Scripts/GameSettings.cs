using UnityEngine;
using XCharts.Runtime;

// ReSharper disable InconsistentNaming

[CreateAssetMenu(fileName = "GameSettings", menuName = "Configs/GameSettings", order = 1)]
public class GameSettings : ScriptableObject
{
    public Theme theme;
    
    public bool devMode = false;
    public bool showDebugLogs = true;
        
    [Header("Adlete Configs")]
    public int adlete_requestInterval = 5;
}