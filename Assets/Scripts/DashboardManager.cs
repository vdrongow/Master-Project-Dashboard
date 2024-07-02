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
        UpdateButtonText();
    }
    
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
}