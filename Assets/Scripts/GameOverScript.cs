using UnityEngine;
using UnityEngine.SceneManagement;


public class GameOverScript : MonoBehaviour
{   
    public void die()
    {
        Loader.Load(Loader.Scene.gameover);
    }
    
    
}
