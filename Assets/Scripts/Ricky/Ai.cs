using UnityEngine;

public class Ai : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public string targetTag = "Player";
    
    [Header("AI Behavior")]
    public float detectionRange = 15f;
    public float attackRange = 2f;
    public float chaseSpeed = 3f;
    public float rotationSpeed = 5f;
    
    [Header("Attack Settings")]
    public float attackCooldown = 2f;
    private float lastAttackTime = 0f;
    public float attackDuration = 1f;
    
    [Header("Ground Settings")]
    public float groundLevel = 0f; // Set this to your ground height
    public bool snapToGround = true;
    
    [Header("Animation")]
    public Animator anim;
    
    private bool isAttacking = false;
    private bool isDead = false;

    public PlayerStats player;
    
    void Start()
    {
        // Get Animator
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
        
        // Find Mario
        if (target == null)
        {
            GameObject mario = GameObject.FindGameObjectWithTag(targetTag);
            if (mario == null) mario = GameObject.Find("mario");
            
            if (mario != null)
            {
                target = mario.transform;
                Debug.Log("Ricky found Mario at: " + target.position);
            }
            else
            {
                Debug.LogWarning("Ricky couldn't find Mario! Make sure Mario has 'Player' tag.");
            }
        }
        
        // Find PlayerStats on Mario
        if (player == null && target != null)
        {
            player = target.GetComponent<PlayerStats>();
            if (player != null)
            {
                Debug.Log("Ricky found PlayerStats component on Mario");
            }
            else
            {
                Debug.LogWarning("Ricky couldn't find PlayerStats on Mario!");
            }
        }
        
        Debug.Log("Ricky initialized at position: " + transform.position);
    }
    
    void Update()
    {
        if (isDead || target == null) return;
        
        float distanceToMario = Vector3.Distance(transform.position, target.position);
        
        // Check if can see Mario
        if (distanceToMario <= detectionRange)
        {
            // Calculate direction to Mario (keep on same Y level)
            Vector3 direction = (target.position - transform.position);
            direction.y = 0;
            
            // Rotate to face Mario
            if (direction.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            
            // Attack if close enough
            if (distanceToMario <= attackRange && !isAttacking)
            {
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    Attack();
                }
                else
                {
                    // Waiting for cooldown - stop moving
                    if (anim != null) anim.SetBool("isWalking", false);
                }
            }
            // Chase if too far
            else if (!isAttacking)
            {
                Chase(direction.normalized);
            }
            else
            {
                // Attacking - stop moving
                if (anim != null) anim.SetBool("isWalking", false);
            }
        }
        else
        {
            // Too far - idle
            if (anim != null) anim.SetBool("isWalking", false);
        }
    }
    
    void Chase(Vector3 direction)
    {
        // Simple movement - just move the transform directly
        Vector3 move = direction * chaseSpeed * Time.deltaTime;
        move.y = 0; // Keep on same height
        transform.position += move;
        
        // Snap to ground level if enabled
        if (snapToGround)
        {
            Vector3 pos = transform.position;
            pos.y = groundLevel;
            transform.position = pos;
        }
        
        // Walk animation
        if (anim != null) anim.SetBool("isWalking", true);
        
        // Debug every 60 frames (about once per second)
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"Ricky chasing! Distance: {Vector3.Distance(transform.position, target.position):F2}");
        }
    }
    
    void Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        
        if (anim != null)
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isAttacking", true);
        }
        
        if (player != null)
        {
            player.TakeDamage(10);
            Debug.Log("RICKY ATTACKING MARIO!");
        }
        else
        {
            Debug.LogWarning("Can't attack - PlayerStats reference is missing!");
        }
        
        Invoke("EndAttack", attackDuration);
    }
    
    void EndAttack()
    {
        isAttacking = false;
        if (anim != null) anim.SetBool("isAttacking", false);
        Debug.Log("Attack ended, back to chasing");
    }
    
    void OnDrawGizmosSelected()
    {
        // Detection range (yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Attack range (red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Line to target
        if (target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}