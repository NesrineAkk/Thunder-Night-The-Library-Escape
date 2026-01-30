using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float maxHealth;

    private float currentHealth;
    private float keys;

    public HealthBar healthBar;
    
    public KeysText keyText;
    public GameOverScript scene;

    void Start()
    {
        keys = 0;
        currentHealth = 100;
        keyText.SetText(keys);
        healthBar.SetSlider(currentHealth);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(20f);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("heart"))
        {
            currentHealth += 20f;
            healthBar.SetSlider(currentHealth);
            if(currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
                healthBar.SetSlider(currentHealth);
            }
            Destroy(other.gameObject);
            Debug.Log("heald, current hp: " + currentHealth);
        }
        if(other.CompareTag("key"))
        {
            keys += 1;
            keyText.SetText(keys);

            Destroy(other.gameObject);
            Debug.Log("key collected, current keys: " + keys);
        }
    }
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        healthBar.SetSlider(currentHealth);
        if(currentHealth <= 0)
        {
            scene.die();
        }
    }

    
}
