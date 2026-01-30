using UnityEngine;
using UnityEngine.SceneManagement;


public class GameOverScript : MonoBehaviour
{   
    public void die()
    {
        SceneManager.LoadScene(1);
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
