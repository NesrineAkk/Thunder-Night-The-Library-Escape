using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class KeyMessageUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;
    public Button closeButton;
    
    [Header("Animation")]
    public float fadeInDuration = 0.5f;
    public float displayDuration = 5f;
    public bool autoClose = true;
    
    private CanvasGroup canvasGroup;
    private bool isShowing = false;
    
    void Start()
    {
        // Get or add CanvasGroup for fading
        if (messagePanel != null)
        {
            canvasGroup = messagePanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = messagePanel.AddComponent<CanvasGroup>();
            }
        }
        
        // Setup close button
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideMessage);
        }
        
        // Hide at start
        HideMessageImmediate();
    }
    
    public void ShowMessage(string message)
    {
        if (isShowing) return;
        
        if (messageText != null)
        {
            messageText.text = message;
        }
        
        if (messagePanel != null)
        {
            messagePanel.SetActive(true);
        }
        
        StartCoroutine(ShowMessageCoroutine());
    }
    
    System.Collections.IEnumerator ShowMessageCoroutine()
    {
        isShowing = true;
        
        // Fade in
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            }
            yield return null;
        }
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
        
        // Wait for display duration
        if (autoClose)
        {
            yield return new WaitForSeconds(displayDuration);
            HideMessage();
        }
    }
    
    public void HideMessage()
    {
        StartCoroutine(HideMessageCoroutine());
    }
    
    System.Collections.IEnumerator HideMessageCoroutine()
    {
        // Fade out
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeInDuration);
            }
            yield return null;
        }
        
        HideMessageImmediate();
        isShowing = false;
    }
    
    void HideMessageImmediate()
    {
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }
}