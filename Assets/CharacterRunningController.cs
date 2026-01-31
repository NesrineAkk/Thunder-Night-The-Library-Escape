using UnityEngine;

public class MarioMovement : MonoBehaviour
{
    public Stamina stamina;
    public PlayerStats stats;

    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float rotationSpeed = 10f;
    
    [Header("Camera Reference")]
    public Transform cameraTransform;
    
    [Header("Animation")]
    public Animator animator;
    
    [Header("Crouch Settings")]
    public KeyCode crouchKey = KeyCode.C;
    public float crouchSpeed = 1.5f;
    
    [Header("Jump Settings")]
    public KeyCode jumpKey = KeyCode.Space;
    
    [Header("Ground Settings")]
    public float groundLevel = 0f; // Set this to your ground height
    public bool snapToGround = true;
    
    [Header("Testing/Debug")]
    public KeyCode takeDamageKey = KeyCode.H; // Press H to take damage (for testing)
    public float testDamageAmount = 10f;
    
    private bool isRunning = false;
    private bool isWalking = false;
    private bool isCrouching = false;
    private float lastMovementTime = 0f;
    private void OnTriggerEnter(Collider other)
    {
        
        if(other.CompareTag("Key"))
        {
            stats.AddKey();
            Destroy(other.gameObject);
        }
    }
    
    void Start()
    {
        stats = PlayerStats.Get();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        
        /* Auto-find health component if not assigned
        if (health == null)
        {
            health = GetComponent<Health>();
        }
        
        // Auto-find stamina component if not assigned
        if (stamina == null)
        {
            stamina = GetComponent<Stamina>();
        }*/
        
        // Auto-find camera if not assigned
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
            if (cameraTransform != null)
            {
                Debug.Log("Camera automatically found: " + cameraTransform.name);
            }
            else
            {
                Debug.LogWarning("No camera found! Movement will be world-space.");
            }
        }
    }
    
    void Update()
    {
        // Don't allow movement if dead
        if (stats.currentHealth <= 0)
        {
            return;
        }
        
        /* Testing: Take damage on key press
        if (Input.GetKeyDown(takeDamageKey) && health != null)
        {
            health.TakeDamage(testDamageAmount);
        }*/
        
        // Handle jump input (just for animation)
        if (Input.GetKeyDown(jumpKey))
        {
            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }
        }
        
        // Get input directly
        float moveX = 0f;
        float moveZ = 0f;
        
        if (Input.GetKey(KeyCode.W)) moveZ = 1f;
        if (Input.GetKey(KeyCode.S)) moveZ = -1f;
        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        if (Input.GetKey(KeyCode.D)) moveX = 1f;
        
        // Create movement vector RELATIVE TO CAMERA
        Vector3 movement = Vector3.zero;
        
        if (cameraTransform != null)
        {
            // Get camera's forward and right directions (ignore Y axis)
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;
            
            // Flatten the vectors (remove Y component to keep movement on ground)
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();
            
            // Calculate movement relative to camera
            movement = (cameraForward * moveZ + cameraRight * moveX).normalized;
        }
        else
        {
            // Fallback to world space if no camera
            movement = new Vector3(moveX, 0f, moveZ).normalized;
        }
        
        // Check if C key is being held
        bool pressingCrouch = Input.GetKey(crouchKey);
        
        // Check if moving
        bool hasMovementInput = movement.magnitude > 0.1f;
        
        // Update last movement time if we have input
        if (hasMovementInput)
        {
            lastMovementTime = Time.time;
        }
        
        // Consider "moving" if we have input OR recently had input
        bool isMoving = hasMovementInput || (Time.time - lastMovementTime < 0.1f);
        
        // Crouch walking
        bool isCrouchWalking = pressingCrouch && (hasMovementInput || (pressingCrouch && Time.time - lastMovementTime < 0.15f));
        isCrouching = isCrouchWalking;
        
        // Running (can't run while crouching, and need stamina)
        bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        isRunning = isMoving && shiftPressed && !pressingCrouch && stamina != null && stamina.currentStamina > 0;
        isWalking = isMoving && !isRunning && !pressingCrouch;
        
        // Choose speed
        float currentSpeed;
        if (isCrouchWalking)
        {
            currentSpeed = crouchSpeed;
            if (stamina != null) stamina.isUsingStamina = false;
        }
        else if (isRunning)
        {
            currentSpeed = runSpeed;
            if (stamina != null) stamina.isUsingStamina = true;
        }
        else
        {
            currentSpeed = walkSpeed;
            if (stamina != null) stamina.isUsingStamina = false;
        }
        
        // Move the character - simple transform movement
        if (isMoving)
        {
            Vector3 move = movement * currentSpeed * Time.deltaTime;
            move.y = 0; // Keep on same height
            transform.position += move;
            
            // Rotate to face direction
            if (movement.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movement);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }
        
        // Snap to ground level if enabled
        if (snapToGround)
        {
            Vector3 pos = transform.position;
            pos.y = groundLevel;
            transform.position = pos;
        }
        
        // Update animation
        if (animator != null)
        {
            animator.SetBool("IsWalking", isWalking);
            animator.SetBool("IsRunning", isRunning);
            animator.SetBool("IsCrouchWalking", isCrouchWalking);
            animator.SetBool("IsGrounded", true); // Always grounded in this simple version
            animator.SetFloat("Speed", isMoving ? currentSpeed : 0f);
        }
    }
}