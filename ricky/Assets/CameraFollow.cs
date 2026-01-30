using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Header("Distance (Zoom)")]
    public float distance = 4f;
    public float minDistance = 2f;
    public float maxDistance = 7f;
    public float zoomSpeed = 2f;

    [Header("Height")]
    public float height = 2.5f;

    [Header("Rotation")]
    public float mouseSensitivity = 3f;
    public float minY = -30f;
    public float maxY = 60f;

    [Header("Smooth")]
    public float followSpeed = 10f;

    [Header("Collision")]
    public bool enableCollision = true;
    public float collisionOffset = 0.3f;
    public LayerMask collisionLayers = -1; // Check all layers by default

    private float yaw;
    private float pitch;
    private float currentDistance;

    void Start()
    {
        if (!target)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                Debug.Log("Camera auto-found target: " + player.name);
            }
            else
            {
                Debug.LogError("Camera has no target!");
                enabled = false;
                return;
            }
        }

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
        currentDistance = distance;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (!target) return;

        // Unlock cursor with ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Re-lock cursor on click
        if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Only rotate if cursor is locked
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, minY, maxY);
        }

        // Zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        // Calculate rotation
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        // Target position for the camera to look at
        Vector3 targetLookPoint = target.position + Vector3.up * height;

        // Calculate desired camera position
        Vector3 offset = rotation * new Vector3(0, 0, -distance);
        Vector3 desiredPos = targetLookPoint + offset;

        // Apply collision detection
        if (enableCollision)
        {
            Vector3 direction = desiredPos - targetLookPoint;
            float desiredDistance = direction.magnitude;

            RaycastHit hit;
            if (Physics.Raycast(targetLookPoint, direction.normalized, out hit, desiredDistance, collisionLayers))
            {
                // Camera hit something, move it closer
                currentDistance = Mathf.Lerp(currentDistance, hit.distance - collisionOffset, followSpeed * Time.deltaTime);
            }
            else
            {
                // No obstruction, return to desired distance
                currentDistance = Mathf.Lerp(currentDistance, distance, followSpeed * Time.deltaTime);
            }

            // Recalculate position with adjusted distance
            offset = rotation * new Vector3(0, 0, -currentDistance);
            desiredPos = targetLookPoint + offset;
        }

        // Smooth follow
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            followSpeed * Time.deltaTime
        );

        // Look at target
        transform.LookAt(targetLookPoint);
    }
}