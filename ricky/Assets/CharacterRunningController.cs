using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class MarioMovement : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent agent;


    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    
    [Header("Animation")]
    public Animator animator;
    
    private CharacterController characterController;
    
    [Header("Crouch Settings")]
    public KeyCode crouchKey = KeyCode.C;
    public float crouchSpeed = 1.5f;
    
    [Header("Jump Settings")]
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    public bool useAnimationJump = true;
    
    private bool isRunning = false;
    private bool isWalking = false;
    private bool isGrounded = true;
    private float verticalVelocity = 0f;
    private bool isCrouching = false;
    private float lastMovementTime = 0f; // Track when we last moved
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("Character Controller not found! Please add one to " + gameObject.name);
        }
    }
    
    void Update()
    {
        HandleJump();
        
        // Get input directly
        float moveX = 0f;
        float moveZ = 0f;
        
        if (Input.GetKey(KeyCode.W)) moveZ = 1f;
        if (Input.GetKey(KeyCode.S)) moveZ = -1f;
        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        if (Input.GetKey(KeyCode.D)) moveX = 1f;
        
        // Create movement vector
        Vector3 movement = new Vector3(moveX, 0f, moveZ).normalized;
        
        // Check if C key is being held (check this FIRST before checking movement)
        bool pressingCrouch = Input.GetKey(crouchKey);
        
        // Check if moving (AFTER we know if crouch is pressed)
        bool hasMovementInput = movement.magnitude > 0.1f;
        
        // Update last movement time if we have input
        if (hasMovementInput)
        {
            lastMovementTime = Time.time;
        }
        
        // Consider "moving" if we have input OR recently had input (0.1 second buffer)
        bool isMoving = hasMovementInput || (Time.time - lastMovementTime < 0.1f);
        
        // Crouch walking: C is held AND moving (with buffer)
        bool isCrouchWalking = pressingCrouch && (hasMovementInput || (pressingCrouch && Time.time - lastMovementTime < 0.15f));
        isCrouching = isCrouchWalking;
        
        // Can't run while crouching
        bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        isRunning = isMoving && shiftPressed && !pressingCrouch;
        isWalking = isMoving && !isRunning && !pressingCrouch;
        
        // Choose speed
        float currentSpeed;
        if (isCrouchWalking)
        {
            currentSpeed = crouchSpeed;
        }
        else if (isRunning)
        {
            currentSpeed = runSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }
        
        // Move the character
        if (isMoving && characterController != null)
        {

            Vector3 move = movement * currentSpeed * Time.deltaTime;
            characterController.Move(move);
            
            // Rotate to face direction
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
        
        // Apply vertical movement (jumping/falling)
        ApplyVerticalMovement();
        
        // Update animation
        if (animator != null)
        {
            animator.SetBool("IsWalking", isWalking);
            animator.SetBool("IsRunning", isRunning);
            animator.SetBool("IsCrouchWalking", isCrouchWalking);
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("Speed", isMoving ? currentSpeed : 0f);
        }
        
        // Debug - MORE DETAILED
        Debug.Log($"moveX: {moveX}, moveZ: {moveZ}, movement.magnitude: {movement.magnitude}, pressingCrouch: {pressingCrouch}, isMoving: {isMoving}, CrouchWalking: {isCrouchWalking}");
    }
    
    void HandleJump()
    {
        // Check if grounded using Character Controller
        if (characterController != null)
        {
            isGrounded = characterController.isGrounded;
            
            // Additional raycast check if Character Controller says not grounded
            if (!isGrounded)
            {
                RaycastHit hit;
                isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, 0.3f);
            }
        }
        else
        {
            // Fallback raycast method
            RaycastHit hit;
            isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f);
        }
        
        // Jump when space is pressed and grounded
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Debug.Log("Jumping!");
            
            if (useAnimationJump)
            {
                // Only trigger animation, let animation handle the movement
                if (animator != null)
                {
                    animator.SetTrigger("Jump");
                }
            }
            else
            {
                // Use physics-based jump
                verticalVelocity = jumpForce;
                if (animator != null)
                {
                    animator.SetTrigger("Jump");
                }
            }
        }
    }
    
    void ApplyVerticalMovement()
    {
        // Only apply gravity/physics if NOT using animation-based jump
        if (!useAnimationJump)
        {
            // Apply gravity
            if (!isGrounded)
            {
                verticalVelocity += gravity * Time.deltaTime;
            }
            else
            {
                // Reset vertical velocity when grounded
                if (verticalVelocity < 0)
                {
                    verticalVelocity = -2f;
                }
            }
            
            // Move character vertically using Character Controller
            if (characterController != null)
            {
                Vector3 verticalMove = Vector3.up * verticalVelocity * Time.deltaTime;
                characterController.Move(verticalMove);
            }
            else
            {
                transform.Translate(Vector3.up * verticalVelocity * Time.deltaTime, Space.World);
            }
        }
        else
        {
            // Apply small constant downward force to maintain ground contact
            if (characterController != null)
            {
                Vector3 downForce = Vector3.down * 2f * Time.deltaTime;
                characterController.Move(downForce);
            }
        }
    }
}