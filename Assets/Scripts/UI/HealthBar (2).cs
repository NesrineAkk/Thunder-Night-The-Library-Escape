using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    private PlayerStats manager;

    void Start(){
        manager = PlayerStats.Get();
        if(manager == null)
        {
            Debug.Log("manager not found");
        }

        SetSliderMax(manager.maxHealth);
    }

    void Update(){
        SetSlider(manager.currentHealth);
    }
    public void SetSlider(float amount)
    {
        healthSlider.value = amount;
    }

    public void SetSliderMax(float amount)
    {
        healthSlider.maxValue = amount;
    }

    
}
