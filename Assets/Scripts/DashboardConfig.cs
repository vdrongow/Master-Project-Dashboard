using UnityEngine;

[CreateAssetMenu(fileName = "DashboardConfig", menuName = "Configs/Dashboard Config", order = 1)]
public sealed class DashboardConfig : ScriptableObject
{
    [Header("Lobby Configs")]
    public string defaultLobbyName = "newSession";
    public int maxLobbySize = 20;
    public bool isPrivateLobby = true;

    public string dashboardScene = "DashboardScene";
    public string loginScene = "Login";
}