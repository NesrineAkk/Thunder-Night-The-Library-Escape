using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;


    [Header("Health")] 
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("Key")] 
    public int keys = 0;



    public HealthBar healthBar;

    public KeysText keyText;

    public GameOverScript scene;

    void Awake()
    {
        Debug.Log("stats manager alive");

        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
    }
    public static PlayerStats Get()
    {
        if (Instance == null)
        {
            GameObject go = new GameObject("PlayerStats");
            Instance = go.AddComponent<PlayerStats>();
            DontDestroyOnLoad(go);
        }
        return Instance;
    }

    public void AddKey()
    {
        keys += 1;
    }

    public void restart()
    {
        currentHealth = 100;
        keys = 0;
        Loader.Load(Loader.Scene.SampleScene);

    }
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if(currentHealth <= 0)
        {
            Loader.Load(Loader.Scene.gameover);

        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        healthBar.SetSlider(currentHealth);
        if(currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
                healthBar.SetSlider(currentHealth);
            }
    }

    
    

    
}
