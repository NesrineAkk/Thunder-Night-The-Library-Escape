using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    [Header("Target")]
    private PlayerStats stats;
    public Transform target;
    public string targetTag = "Player";
    
    [Header("AI Behavior")]
    public float detectionRange = 15f;
    public float attackRange = 2f;
    public float chaseSpeed = 3f;
    public float wanderSpeed = 1.5f;
    public float rotationSpeed = 5f;
    
    [Header("Wandering")]
    public float wanderRadius = 10f;
    public float wanderTimer = 5f;
    private float wanderCooldown;
    private Vector3 wanderTarget;
    
    [Header("Attack Settings")]
    public float attackCooldown = 2f;
    private float lastAttackTime = 0f;
    public float attackDuration = 1f;
    public float attackDamage = 10f; // NEW: Damage dealt to Mario
    
    [Header("Physics")]
    public float gravity = -20f;
    private Vector3 velocity;
    
    [Header("Animation")]
    public Animator anim;
    
    private CharacterController characterController;
    private bool isAttacking = false;
    private bool isDead = false;
    private Vector3 startPosition;
    
    // AI States
    private enum AIState { Wandering, Chasing, Attacking, Idle }
    private AIState currentState = AIState.Wandering;
    
    void Start()
    {
        stats = PlayerStats.Get();
        // Get or add Character Controller
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
            Debug.Log("Added Character Controller to zombie");
        }
        
        // Configure Character Controller
        characterController.radius = 0.5f;
        characterController.height = 2f;
        characterController.center = new Vector3(0, 1, 0);
        characterController.slopeLimit = 45f;
        characterController.stepOffset = 0.3f;
        
        // Remove Rigidbody if it exists
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Destroy(rb);
            Debug.Log("Removed Rigidbody from zombie");
        }
        
        // Get Animator
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
        
        // Find Mario
        if (target == null)
        {
            GameObject mario = GameObject.FindGameObjectWithTag(targetTag);
            if (mario == null) mario = GameObject.Find("Mario2");
            
            if (mario != null)
            {
                target = mario.transform;
                Debug.Log("Zombie found Mario!");
            }
            else
            {
                Debug.LogWarning("Zombie couldn't find Mario! Make sure Mario has 'Player' tag.");
            }
        }
        
        // Store starting position for wandering
        startPosition = transform.position;
        
        // Initialize wandering
        wanderCooldown = wanderTimer;
        SetNewWanderTarget();
        
        Debug.Log("Zombie initialized at position: " + transform.position);
    }
    
    void Update()
    {
        if (isDead) return;
        
        // Apply gravity
        if (characterController.isGrounded)
        {
            velocity.y = -2f; // Small downward force to keep grounded
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
        characterController.Move(velocity * Time.deltaTime);
        
        // Check distance to Mario
        float distanceToMario = float.MaxValue;
        if (target != null)
        {
            distanceToMario = Vector3.Distance(transform.position, target.position);
        }
        
        // State Machine
        if (target != null && distanceToMario <= attackRange && !isAttacking)
        {
            // ATTACKING STATE
            currentState = AIState.Attacking;
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
            }
            else
            {
                if (anim != null) anim.SetBool("isWalking", false);
            }
        }
        else if (target != null && distanceToMario <= detectionRange)
        {
            // CHASING STATE
            currentState = AIState.Chasing;
            if (!isAttacking)
            {
                Vector3 direction = (target.position - transform.position);
                direction.y = 0;
                
                // Rotate to face Mario
                if (direction.magnitude > 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
                
                Chase(direction.normalized);
            }
            else
            {
                if (anim != null) anim.SetBool("isWalking", false);
            }
        }
        else
        {
            // WANDERING STATE
            if (!isAttacking)
            {
                currentState = AIState.Wandering;
                Wander();
            }
        }
    }
    
    void Wander()
    {
        // Check if reached wander target
        Vector3 directionToWander = wanderTarget - transform.position;
        directionToWander.y = 0;
        float distanceToWander = directionToWander.magnitude;
        
        if (distanceToWander < 1f)
        {
            // Reached target, wait then pick new target
            wanderCooldown -= Time.deltaTime;
            if (wanderCooldown <= 0)
            {
                SetNewWanderTarget();
                wanderCooldown = wanderTimer;
            }
            
            if (anim != null) anim.SetBool("isWalking", false);
        }
        else
        {
            // Move towards wander target
            Vector3 direction = directionToWander.normalized;
            
            // Rotate towards target
            if (direction.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            
            // Move
            Vector3 move = direction * wanderSpeed * Time.deltaTime;
            characterController.Move(move);
            
            if (anim != null) anim.SetBool("isWalking", true);
        }
    }
    
    void SetNewWanderTarget()
    {
        // Pick a random point around the starting position
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        wanderTarget = startPosition + new Vector3(randomCircle.x, 0, randomCircle.y);
        
        Debug.Log("Zombie picked new wander target: " + wanderTarget);
    }
    
    void Chase(Vector3 direction)
    {
        if (characterController == null) return;
        
        // Move towards Mario
        Vector3 move = direction * chaseSpeed * Time.deltaTime;
        characterController.Move(move);
        
        // Walk animation
        if (anim != null) anim.SetBool("isWalking", true);
        
        Debug.Log("Zombie chasing! Distance: " + Vector3.Distance(transform.position, target.position));
    }
    
    void Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        
        if (anim != null)
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isHitting", true);
        }
        
        Debug.Log("ZOMBIE ATTACKING MARIO!");
        
        /* Deal damage to Mario
        if (target != null)
        {
            Health marioHealth = target.GetComponent<Health>();
            if (marioHealth != null)
            {
                marioHealth.TakeDamage(attackDamage);
                Debug.Log($"Zombie dealt {attackDamage} damage to Mario! Mario's health: {marioHealth.currentHealth}/{marioHealth.maxHealth}");
            }
            else
            {
                Debug.LogWarning("Mario doesn't have a Health component!");
            }
        }*/
        stats.TakeDamage(attackDamage);
        
        Invoke("EndAttack", attackDuration);
    }
    
    void EndAttack()
    {
        isAttacking = false;
        if (anim != null) anim.SetBool("isHitting", false);
        Debug.Log("Attack ended, back to behavior");
    }
    
    public void Die()
    {
        isDead = true;
        if (anim != null)
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isHitting", false);
            anim.SetTrigger("isDying");
        }
        
        // Disable Character Controller
        if (characterController != null)
        {
            characterController.enabled = false;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Detection range (yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Attack range (red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Wander radius (blue) - from start position
        Gizmos.color = Color.blue;
        Vector3 drawPos = Application.isPlaying ? startPosition : transform.position;
        Gizmos.DrawWireSphere(drawPos, wanderRadius);
        
        // Current wander target (cyan)
        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(wanderTarget, 0.5f);
            Gizmos.DrawLine(transform.position, wanderTarget);
        }
        
        // Line to target
        if (target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}