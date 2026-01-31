using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    
    [Header("Regeneration (Optional)")]
    public bool canRegenerate = false;
    public float healthRegenRate = 5f; // Health per second
    public float regenDelay = 3f; // Delay after taking damage before regen starts
    
    [Header("Events")]
    public UnityEvent onDeath;
    public UnityEvent<float> onHealthChanged; // Passes current health percentage
    
    private float lastDamageTime;
    private bool isDead = false;
    
    void Start()
    {
        // Initialize health to max
        currentHealth = maxHealth;
    }
    
    void Update()
    {
        // Handle health regeneration if enabled
        if (canRegenerate && currentHealth < maxHealth && !isDead)
        {
            if (Time.time - lastDamageTime >= regenDelay)
            {
                Heal(healthRegenRate * Time.deltaTime);
            }
        }
    }
    
    // Take damage
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        lastDamageTime = Time.time;
        
        // Trigger health changed event
        onHealthChanged?.Invoke(GetHealthPercentage());
        
        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
        
        Debug.Log($"Took {damage} damage. Current health: {currentHealth}/{maxHealth}");
    }
    
    // Heal
    public void Heal(float amount)
    {
        if (isDead) return;
        
        currentHealth += amount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);
        
        // Trigger health changed event
        onHealthChanged?.Invoke(GetHealthPercentage());
    }
    
    // Get health as a percentage (0 to 1)
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
    
    // Check if alive
    public bool IsAlive()
    {
        return !isDead;
    }
    
    // Die
    void Die()
    {
        if (isDead) return;
        
        isDead = true;
        Debug.Log("Player has died!");
        
        // Trigger death event
        onDeath?.Invoke();
        
        // Disable movement
        var movement = GetComponent<MarioMovement>();
        if (movement != null)
        {
            movement.enabled = false;
        }
        
        // Show Game Over screen
        GameOverManager gameOverManager = FindObjectOfType<GameOverManager>();
        if (gameOverManager != null)
        {
            gameOverManager.ShowGameOver();
        }
        else
        {
            Debug.LogWarning("No GameOverManager found in scene!");
        }
    }
    
    // Respawn/Reset
    public void Respawn()
    {
        isDead = false;
        currentHealth = maxHealth;
        
        // Re-enable movement
        var movement = GetComponent<MarioMovement>();
        if (movement != null)
        {
            movement.enabled = true;
        }
        
        onHealthChanged?.Invoke(GetHealthPercentage());
        Debug.Log("Player respawned!");
    }
}