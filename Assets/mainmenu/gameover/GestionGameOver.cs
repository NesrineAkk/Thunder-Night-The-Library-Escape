using UnityEngine;
using UnityEngine.SceneManagement;

public class GestionGameOver : MonoBehaviour
{
    public void RetourMenu()
    {
        PlayerStats.Get().restart();
        
        
    }
}