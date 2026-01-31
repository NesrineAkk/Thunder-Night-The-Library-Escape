using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyCounterUI : MonoBehaviour
{
    [Header("References")]
    public KeyCollector keyCollector;
    
    [Header("UI Elements")]
    public TextMeshProUGUI keyCountText;
    public Image keyIcon;
    
    [Header("Display Format")]
    public string textFormat = "Keys: {0}/{1}";
    public bool showIcon = true;
    
    [Header("Animation (Optional)")]
    public bool animateOnCollect = true;
    public float scaleAmount = 1.3f;
    public float animationDuration = 0.3f;
    
    private Vector3 originalScale;
    private bool isAnimating = false;
    
    void Start()
    {
        // Auto-find KeyCollector if not assigned
        if (keyCollector == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                keyCollector = player.GetComponent<KeyCollector>();
            }
        }
        
        // Store original scale for animation
        if (keyCountText != null)
        {
            originalScale = keyCountText.transform.localScale;
        }
        
        // Show/hide icon
        if (keyIcon != null)
        {
            keyIcon.enabled = showIcon;
        }
        
        // Initial update
        UpdateDisplay();
    }
    
    void Update()
    {
        UpdateDisplay();
    }
    
    void UpdateDisplay()
    {
        if (keyCollector == null || keyCountText == null) return;
        
        // Update text
        string displayText = string.Format(textFormat, 
            keyCollector.keysCollected, 
            keyCollector.totalKeysNeeded);
        
        if (keyCountText.text != displayText)
        {
            keyCountText.text = displayText;
            
            // Animate when key count changes
            if (animateOnCollect && !isAnimating)
            {
                StartCoroutine(AnimateKeyCollection());
            }
        }
    }
    
    System.Collections.IEnumerator AnimateKeyCollection()
    {
        if (keyCountText == null) yield break;
        
        isAnimating = true;
        float elapsed = 0f;
        
        // Scale up
        while (elapsed < animationDuration / 2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (animationDuration / 2);
            keyCountText.transform.localScale = Vector3.Lerp(originalScale, originalScale * scaleAmount, t);
            yield return null;
        }
        
        elapsed = 0f;
        
        // Scale back down
        while (elapsed < animationDuration / 2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (animationDuration / 2);
            keyCountText.transform.localScale = Vector3.Lerp(originalScale * scaleAmount, originalScale, t);
            yield return null;
        }
        
        keyCountText.transform.localScale = originalScale;
        isAnimating = false;
    }
}