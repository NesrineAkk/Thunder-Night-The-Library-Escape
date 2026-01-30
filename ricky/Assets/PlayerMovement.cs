using UnityEngine;

public class ZombieAI : MonoBehaviour
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
    
    [Header("Animation")]
    public Animator anim;
    
    private CharacterController characterController;
    private bool isAttacking = false;
    private bool isDead = false;
    
    void Start()
    {
        // Get or add Character Controller (same as Mario)
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
            Debug.Log("Added Character Controller to zombie");
        }
        
        // Configure Character Controller (same settings as Mario)
        characterController.radius = 0.5f;
        characterController.height = 2f;
        characterController.center = new Vector3(0, 1, 0);
        characterController.slopeLimit = 45f;
        characterController.stepOffset = 0.3f;
        
        // Remove Rigidbody if it exists (Mario doesn't have one)
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
        
        Debug.Log("Zombie initialized at position: " + transform.position);
    }
    
    void Update()
    {
        if (isDead || target == null) return;
        
        float distanceToMario = Vector3.Distance(transform.position, target.position);
        
        // Check if can see Mario
        if (distanceToMario <= detectionRange)
        {
            // Calculate direction to Mario
            Vector3 direction = (target.position - transform.position);
            direction.y = 0; // Keep on same level
            
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
        
        // NO GRAVITY APPLICATION - just like Mario's script!
        // The CharacterController handles ground detection automatically
    }
    
    void Chase(Vector3 direction)
    {
        if (characterController == null) return;
        
        // Move towards Mario (same method as Mario's movement)
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
        
        Invoke("EndAttack", attackDuration);
    }
    
    void EndAttack()
    {
        isAttacking = false;
        if (anim != null) anim.SetBool("isHitting", false);
        Debug.Log("Attack ended, back to chasing");
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
        
        // Line to target
        if (target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}