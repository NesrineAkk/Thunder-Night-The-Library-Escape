using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class RickyAi : MonoBehaviour
{
    // ---------------- COMPONENTS ----------------
    private NavMeshAgent agent;
    private Animator animator;

    // ---------------- REFERENCES ----------------
    public Transform player;

    // ---------------- HEALTH ----------------
    public float health = 100f;

    // ---------------- RANGES ----------------
    public float sightRange;
    public float attackRange;
    public float attackBuffer = 0.5f;

    // ---------------- PATROL ----------------
    public float walkPointRange = 10f;
    private Vector3 walkPoint;
    private bool walkPointSet;

    // ---------------- ATTACK ----------------
    public float timeBetweenAttacks = 1.5f;
    private bool alreadyAttacked;

    // ---------------- STATES ----------------
    private enum State
    {
        Patrolling,
        Chasing,
        Attacking
    }

    private State currentState;

    private Vector3 lastPlayerPos;
    private Vector3 playerVelocity;


    private float chaseUpdateTime = 0.25f;
    private float chaseTimer = 0f;
    // =========================================================

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (player != null)
        lastPlayerPos = player.position;


        // Force enemy onto NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
        else
        {
            Debug.LogError("Ricky AI: No NavMesh found near enemy!");
        }
        agent.updateRotation = true;
        agent.isStopped = false;
        currentState = State.Patrolling;
    }

    // =========================================================

    private void Update()
{
    if (!agent.isOnNavMesh || player == null)
        return;

    // --- update player velocity for prediction ---
    playerVelocity = (player.position - lastPlayerPos) / Time.deltaTime;
    lastPlayerPos = player.position;

    // --- calculate distance ---
    float distance = Vector3.Distance(transform.position, player.position);

    // --- decide state based on distance ---
    if (distance > sightRange)
        currentState = State.Patrolling;
    else if (distance > attackRange)
        currentState = State.Chasing;
    else
        currentState = State.Attacking;

    // --- execute behavior ---
    switch (currentState)
    {
        case State.Patrolling:
            agent.isStopped = false;
            Patrol();
            break;

        case State.Chasing:
            agent.isStopped = false;
            ChasePlayer();
            break;

        case State.Attacking:
            agent.isStopped = true;
            AttackPlayer();
            break;
    }

    // --- animation ---
    animator.SetFloat("Speed", agent.velocity.magnitude);
}


    // =========================================================
    // ---------------- STATE MANAGEMENT ----------------

    private void SwitchState(State newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;

        switch (newState)
        {
            case State.Patrolling:
                agent.isStopped = false;
                walkPointSet = false;
                break;

            case State.Chasing:
                agent.isStopped = false;
                break;

            case State.Attacking:
                agent.isStopped = true;
                break;
        }
    }

    // =========================================================
    // ---------------- PATROL ----------------

    private void Patrol()
    {
        if (!walkPointSet)
            SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        if (Vector3.Distance(transform.position, walkPoint) < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        float randomZ = Random.Range(-walkPointRange, walkPointRange);

        Vector3 candidate = new Vector3(
            transform.position.x + randomX,
            transform.position.y,
            transform.position.z + randomZ
        );

        NavMeshHit hit;
        if (NavMesh.SamplePosition(candidate, out hit, 2f, NavMesh.AllAreas))
        {
            walkPoint = hit.position;
            walkPointSet = true;
        }
    }

    // =========================================================
    // ---------------- CHASE ----------------

    void ChasePlayer()
{
    if (player == null) return;

    Vector3 predictedPos = player.position + playerVelocity * 0.5f;

    NavMeshHit hit;
    if (NavMesh.SamplePosition(predictedPos, out hit, 1f, NavMesh.AllAreas))
    {
        agent.SetDestination(hit.position);
    }
    else
    {
        agent.SetDestination(player.position);
    }
}



    // =========================================================
    // ---------------- ATTACK ----------------

private void AttackPlayer()
{
    if (player == null) return;

    // Face the player smoothly
    Vector3 direction = player.position - transform.position;
    direction.y = 0f;

    if (direction != Vector3.zero)
    {
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(direction),
            Time.deltaTime * 6f
        );
    }

    // Attack cooldown
    if (!alreadyAttacked)
    {
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Attack");

        alreadyAttacked = true;
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }
}



    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    // =========================================================
    // ---------------- DAMAGE ----------------

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0f)
            Destroy(gameObject, 0.5f);
    }

    // =========================================================
    // ---------------- DEBUG ----------------

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
