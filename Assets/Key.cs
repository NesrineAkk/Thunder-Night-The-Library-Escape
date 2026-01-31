using UnityEngine;

public class Key : MonoBehaviour
{
    [Header("Settings")]
    public bool autoRotate = true;
    public float rotationSpeed = 100f;
    public bool autoFloat = true;
    public float floatAmplitude = 0.3f;
    public float floatSpeed = 2f;
    
    [Header("Effects (played by KeyCollector)")]
    public GameObject pickupEffect;
    public AudioClip pickupSound;
    
    private Vector3 startPosition;
    private float floatTimer = 0f;
    
    void Start()
    {
        startPosition = transform.position;
        
        // Random starting point for float animation
        floatTimer = Random.Range(0f, Mathf.PI * 2);
        
        // Make sure the key has the "Key" tag
        if (!CompareTag("Key"))
        {
            Debug.LogWarning($"{gameObject.name} doesn't have 'Key' tag! Adding it now.");
            gameObject.tag = "Key";
        }
    }
    
    void Update()
    {
        // Rotate the key
        if (autoRotate)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
        
        // Float up and down
        if (autoFloat)
        {
            floatTimer += Time.deltaTime * floatSpeed;
            float newY = startPosition.y + Mathf.Sin(floatTimer) * floatAmplitude;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
    
    // Public method so KeyCollector can access effects
    public void PlayPickupEffects(Vector3 position)
    {
        // Spawn pickup effect
        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, position, Quaternion.identity);
        }
        
        // Play pickup sound
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, position);
        }
    }
}