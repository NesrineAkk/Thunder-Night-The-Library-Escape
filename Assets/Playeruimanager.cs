/*using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIManager : MonoBehaviour
{
    [Header("References")]
    public Health playerHealth;
    public Stamina playerStamina;
    
    [Header("Health UI")]
    public Image healthBarFill;
    public TextMeshProUGUI healthText;
    public Color healthColor = Color.green;
    public Color lowHealthColor = Color.red;
    public float lowHealthThreshold = 0.25f;
    
    [Header("Stamina UI")]
    public Image staminaBarFill;
    public TextMeshProUGUI staminaText;
    public Color staminaColor = new Color(1f, 0.92f, 0.016f); // Yellow
    public Color lowStaminaColor = Color.red;
    public float lowStaminaThreshold = 0.25f;
    
    [Header("Bar Animation")]
    public bool smoothTransition = true;
    public float transitionSpeed = 5f;
    
    private float targetHealthFill = 1f;
    private float targetStaminaFill = 1f;
    
    void Start()
    {
        // Auto-find player if not assigned
        if (playerHealth == null || playerStamina == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                if (playerHealth == null)
                    playerHealth = player.GetComponent<Health>();
                if (playerStamina == null)
                    playerStamina = player.GetComponent<Stamina>();
            }
        }
        
        // Initialize bars
        UpdateHealthBar();
        UpdateStaminaBar();
    }
    
    void Update()
    {
        UpdateHealthBar();
        UpdateStaminaBar();
    }
    
    void UpdateHealthBar()
    {
        if (playerHealth == null || healthBarFill == null) return;
        
        float healthPercentage = playerHealth.GetHealthPercentage();
        targetHealthFill = healthPercentage;
        
        // Smooth or instant fill
        if (smoothTransition)
        {
            healthBarFill.fillAmount = Mathf.Lerp(healthBarFill.fillAmount, targetHealthFill, Time.deltaTime * transitionSpeed);
        }
        else
        {
            healthBarFill.fillAmount = targetHealthFill;
        }
        
        // Update color based on health
        if (healthPercentage <= lowHealthThreshold)
        {
            healthBarFill.color = lowHealthColor;
        }
        else
        {
            healthBarFill.color = healthColor;
        }
        
        // Update text if available
        if (healthText != null)
        {
            healthText.text = $"{Mathf.RoundToInt(playerHealth.currentHealth)}/{Mathf.RoundToInt(playerHealth.maxHealth)}";
        }
    }
    
    void UpdateStaminaBar()
    {
        if (playerStamina == null || staminaBarFill == null) return;
        
        float staminaPercentage = playerStamina.GetStaminaPercentage();
        targetStaminaFill = staminaPercentage;
        
        // Smooth or instant fill
        if (smoothTransition)
        {
            staminaBarFill.fillAmount = Mathf.Lerp(staminaBarFill.fillAmount, targetStaminaFill, Time.deltaTime * transitionSpeed);
        }
        else
        {
            staminaBarFill.fillAmount = targetStaminaFill;
        }
        
        // Update color based on stamina
        if (staminaPercentage <= lowStaminaThreshold)
        {
            staminaBarFill.color = lowStaminaColor;
        }
        else
        {
            staminaBarFill.color = staminaColor;
        }
        
        // Update text if available
        if (staminaText != null)
        {
            staminaText.text = $"{Mathf.RoundToInt(playerStamina.currentStamina)}/{Mathf.RoundToInt(playerStamina.maxStamina)}";
        }
    }
}*/