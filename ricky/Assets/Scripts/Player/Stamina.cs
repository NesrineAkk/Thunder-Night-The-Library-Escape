using UnityEngine;

public class Stamina : MonoBehaviour
{
    [Header("Stamina")]
    public StaminaBar staminaBar;
    public float currentStamina;
    private float maxStamina = 100f;
    public float drainPerSecond = 20f;
    public float regenPerSEcond = 10f;

    public bool isUsingStamina = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentStamina = maxStamina;
        staminaBar.SetSliderMax(maxStamina);
    }

    // Update is called once per frame
    void Update()
    {
        if(isUsingStamina)
        {
            currentStamina -= drainPerSecond * Time.deltaTime;
            //staminaBar.SetSlider(currentStamina);
        }else {
            currentStamina += regenPerSEcond * Time.deltaTime;
            
        }

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        staminaBar.SetSlider(currentStamina);
    }

    
}
