using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Helper script to automatically configure a UI Image as a filled bar.
/// Just attach this to your health/stamina bar fill images and it will set them up correctly.
/// Remove this script after setup is complete.
/// </summary>
[RequireComponent(typeof(Image))]
public class SetupBarFill : MonoBehaviour
{
    void Start()
    {
        Image img = GetComponent<Image>();
        
        // Configure the image as a filled horizontal bar
        img.type = Image.Type.Filled;
        img.fillMethod = Image.FillMethod.Horizontal;
        img.fillOrigin = (int)Image.OriginHorizontal.Left;
        img.fillAmount = 1f;
        
        Debug.Log($"Bar '{gameObject.name}' configured successfully as Filled Horizontal!");
        Debug.Log("You can now remove the SetupBarFill script from this GameObject.");
    }
}