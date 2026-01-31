using UnityEngine;
using UnityEngine.UI;


public class StaminaBar : MonoBehaviour
{
    public Slider staminaSlider;

    public void SetSlider(float amount)
    {
        staminaSlider.value = amount;
    }

    public void SetSliderMax(float amount)
    {
        staminaSlider.maxValue = amount;
        SetSlider(amount);
    }
}
