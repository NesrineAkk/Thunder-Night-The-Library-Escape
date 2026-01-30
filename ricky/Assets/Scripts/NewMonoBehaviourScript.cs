using UnityEngine;

public class CameraFollowFixed : MonoBehaviour
{
    [Header("Target Player")]
    public Transform target;

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0f, 5f, -7f); // relative to player
    public float smoothSpeed = 0.1f; // camera follow smoothness

    [Header("Zoom Settings")]
    public float minDistance = 3f;   // closest the camera can get
    public float maxDistance = 12f;  // farthest
    public float zoomSpeed = 5f;

    private float currentDistance;

    void Start()
    {
        if (target == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) target = playerObj.transform;
        }

        currentDistance = -offset.z; // start distance based on offset
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Handle zoom input (mouse wheel)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            currentDistance -= scroll * zoomSpeed;
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
        }

        // Calculate desired position: always behind the player
        Vector3 desiredPosition = target.position 
                                - target.forward * currentDistance
                                + Vector3.up * offset.y;

        // Smoothly move camera
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Look at the player
        transform.LookAt(target.position + Vector3.up * 1.5f); // adjust 1.5 for head height
    }
}
