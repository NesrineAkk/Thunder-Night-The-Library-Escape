using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public Button restartButton;
    public Button quitButton;
    
    [Header("Settings")]
    public string gameOverMessage = "GAME OVER";
    public float delayBeforeShow = 1f;
    
    private bool isGameOver = false;
    
    void Start()
    {
        // Hide game over panel at start
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        // Setup buttons
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
        
        // Set game over text
        if (gameOverText != null)
        {
            gameOverText.text = gameOverMessage;
        }
    }
    
    public void ShowGameOver()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        
        // Pause the game (optional)
        // Time.timeScale = 0f;
        
        // Show game over panel after delay
        Invoke("DisplayGameOverPanel", delayBeforeShow);
        
        Debug.Log("Game Over!");
    }
    
    void DisplayGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
    
    public void RestartGame()
    {
        // Resume time if it was paused
        Time.timeScale = 1f;
        
        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}