using UnityEngine;
using UnityEngine.Events;

public class KeyCollector : MonoBehaviour
{
    [Header("Key Collection")]
    public int keysCollected = 0;
    public int totalKeysNeeded = 3;
    
    [Header("Keyboard Settings")]
    public KeyCode collectKey = KeyCode.E;
    public float collectionRange = 2f;
    
    [Header("Messages")]
    public KeyMessageUI messageUI;
    public string lastKeyMessage = "This is your last key! Now you can return and leave the library.";
    
    [Header("Events")]
    public UnityEvent<int> onKeyCollected;
    public UnityEvent onAllKeysCollected;
    
    [Header("Audio (Optional)")]
    public AudioClip keyPickupSound;
    private AudioSource audioSource;
    
    [Header("UI Prompt (Optional)")]
    public GameObject promptUI;
    
    private GameObject nearestKey;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && keyPickupSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }
        
        // Auto-find KeyMessageUI if not assigned
        if (messageUI == null)
        {
            messageUI = FindObjectOfType<KeyMessageUI>();
        }
    }
    
    void Update()
    {
        FindNearestKey();
        
        if (promptUI != null)
        {
            promptUI.SetActive(nearestKey != null);
        }
        
        if (Input.GetKeyDown(collectKey) && nearestKey != null)
        {
            CollectKey(nearestKey);
        }
    }
    
    void FindNearestKey()
    {
        GameObject[] allKeys = GameObject.FindGameObjectsWithTag("Key");
        
        nearestKey = null;
        float nearestDistance = collectionRange;
        
        foreach (GameObject key in allKeys)
        {
            float distance = Vector3.Distance(transform.position, key.transform.position);
            
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestKey = key;
            }
        }
    }
    
    public void CollectKey(GameObject key)
    {
        keysCollected++;
        
        Debug.Log($"Key collected! Total: {keysCollected}/{totalKeysNeeded}");
        
        // Play effects from the Key component if it has one
        Key keyComponent = key.GetComponent<Key>();
        if (keyComponent != null)
        {
            keyComponent.PlayPickupEffects(key.transform.position);
        }
        
        // Play sound from KeyCollector as backup
        if (audioSource != null && keyPickupSound != null)
        {
            audioSource.PlayOneShot(keyPickupSound);
        }
        
        // Destroy the key
        Destroy(key);
        nearestKey = null;
        
        onKeyCollected?.Invoke(keysCollected);
        
        // Show message when all keys collected
        if (keysCollected >= totalKeysNeeded)
        {
            Debug.Log("All keys collected!");
            
            // Show the popup message
            if (messageUI != null)
            {
                messageUI.ShowMessage(lastKeyMessage);
            }
            
            onAllKeysCollected?.Invoke();
        }
    }
    
    public bool HasAllKeys()
    {
        return keysCollected >= totalKeysNeeded;
    }
    
    public int GetKeyCount()
    {
        return keysCollected;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, collectionRange);
    }
}