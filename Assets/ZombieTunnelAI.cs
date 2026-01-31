using UnityEngine;

public class ZombieTunnelAI : MonoBehaviour
{
    private PlayerStats stats;

    [Header("Target")]
    public Transform target;
    public string targetTag = "Player";
    
    [Header("AI Behavior")]
    public float detectionRange = 15f;
    public float attackRange = 2.5f;
    public float chaseSpeed = 3f;
    public float rotationSpeed = 5f;
    public float stopDistance = 2f;
    
    [Header("Attack Settings")]
    public float attackCooldown = 2f;
    private float lastAttackTime = 0f;
    public float attackDuration = 1f;
    public float attackDamage = 10f; // NEW: Damage dealt to Mario
    
    [Header("Animation")]
    public Animator anim;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] zombieIdleSounds;      // Groaning/moaning sounds
    public AudioClip[] zombieAttackSounds;    // Attack grunt sounds
    public AudioClip zombieDeathSound;        // Death sound
    public float idleSoundInterval = 5f;      // How often zombie makes idle sounds
    private float lastIdleSoundTime = 0f;
    
    private CharacterController characterController;
    private bool isAttacking = false;
    private bool isDead = false;

    
    
    void Start()
    {
        stats = PlayerStats.Instance;
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
        
        // Get or add AudioSource
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                Debug.Log("Added AudioSource to zombie");
            }
        }
        
        // Configure AudioSource for 3D sound
        audioSource.spatialBlend = 1.0f;  // Full 3D sound
        audioSource.minDistance = 5f;
        audioSource.maxDistance = 30f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        
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
        
        // Play idle sounds occasionally
        if (Time.time >= lastIdleSoundTime + idleSoundInterval)
        {
            PlayIdleSound();
            lastIdleSoundTime = Time.time;
        }
        
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
            // Chase if too far but stop before getting too close
            else if (!isAttacking && distanceToMario > stopDistance)
            {
                Chase(direction.normalized);
            }
            else if (!isAttacking)
            {
                // In the stop zone - don't move
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
        
        // Play attack sound
        PlayAttackSound();
        
        Debug.Log("ZOMBIE ATTACKING MARIO!");
        
        /* NEW: Deal damage to Mario
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
                Debug.LogWarning("Mario doesn't have a Health component! Make sure to add the Health script to Mario.");
            }
        }
        */

        stats.TakeDamage(attackDamage);
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
        
        // Play death sound
        PlayDeathSound();
        
        // Disable Character Controller
        if (characterController != null)
        {
            characterController.enabled = false;
        }
    }
    
    // Audio Methods
    void PlayIdleSound()
    {
        if (audioSource != null && zombieIdleSounds != null && zombieIdleSounds.Length > 0)
        {
            AudioClip clip = zombieIdleSounds[Random.Range(0, zombieIdleSounds.Length)];
            audioSource.PlayOneShot(clip);
        }
    }
    
    void PlayAttackSound()
    {
        if (audioSource != null && zombieAttackSounds != null && zombieAttackSounds.Length > 0)
        {
            AudioClip clip = zombieAttackSounds[Random.Range(0, zombieAttackSounds.Length)];
            audioSource.PlayOneShot(clip);
        }
    }
    
    void PlayDeathSound()
    {
        if (audioSource != null && zombieDeathSound != null)
        {
            audioSource.PlayOneShot(zombieDeathSound);
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
        
        // Stop distance (blue)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
        
        // Line to target
        if (target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}