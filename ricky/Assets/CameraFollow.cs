using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // The character to follow (Mario)
    
    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0f, 5f, -10f); // Camera position relative to target
    public float smoothSpeed = 0.125f; // How smooth the camera follows
    public bool lookAtTarget = true; // Should camera always look at the target?
    
    [Header("Optional: Mouse Look")]
    public bool enableMouseLook = false;
    public float mouseSensitivity = 2f;
    
    private float rotationX = 0f;
    private float rotationY = 0f;
    
    void Start()
    {
        // If no target assigned, try to find Mario
        if (target == null)
        {
            GameObject mario = GameObject.Find("Mario2");
            if (mario != null)
            {
                target = mario.transform;
                Debug.Log("Camera target set to: " + mario.name);
            }
            else
            {
                Debug.LogError("No target found! Please assign a target in the Inspector.");
            }
        }
        
        // Initialize rotation if using mouse look
        if (enableMouseLook)
        {
            Vector3 rot = transform.eulerAngles;
            rotationX = rot.x;
            rotationY = rot.y;
        }
    }
    
    void LateUpdate()
    {
        if (target == null) return;
        
        if (enableMouseLook)
        {
            // Mouse look camera
            FollowWithMouseLook();
        }
        else
        {
            // Simple follow camera
            FollowTarget();
        }
    }
    
    void FollowTarget()
    {
        // Calculate desired position
        Vector3 desiredPosition = target.position + offset;
        
        // Smoothly move camera to desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        
        // Look at the target
        if (lookAtTarget)
        {
            transform.LookAt(target);
        }
    }
    
    void FollowWithMouseLook()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f); // Limit vertical rotation
        
        // Calculate rotation
        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0f);
        
        // Calculate position based on rotation and offset
        Vector3 desiredPosition = target.position - (rotation * Vector3.forward * offset.magnitude);
        desiredPosition.y = target.position.y + offset.y;
        
        // Apply smoothing
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        
        // Look at target
        transform.LookAt(target.position + Vector3.up * offset.y);
    }
}