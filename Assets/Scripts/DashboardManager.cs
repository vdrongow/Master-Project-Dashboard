using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DashboardManager : MonoBehaviour
{
    [SerializeField]
    private Button pauseGameButton;
    
    private void Awake()
    {
        pauseGameButton.onClick.AddListener(PauseGame);
        pauseGameButton.GetComponentInChildren<TextMeshProUGUI>().text = "Pause Game";
    }
    
    private void PauseGame()
    {
        var gameManager = GameManager.Singleton;
    }
}