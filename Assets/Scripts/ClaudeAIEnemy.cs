using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("AI Behavior Settings")]
    public float detectionRange = 10f;
    public float attackRange = 1f;
    public float wanderRadius = 5f;
    public float wanderTimer = 3f;
    public float attackCooldown = 2f;

    [Header("References")]
    public Animator animator;

    private NavMeshAgent agent;
    private Transform player;
    private Vector3 startPosition;
    private float timer;
    private float lastAttackTime;

    // AI States
    private enum AIState
    {
        Idle,
        Wandering,
        Following,
        Attacking
    }

    private AIState currentState = AIState.Idle;
    private Vector3 lastPosition;
    private bool isMoving = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        startPosition = transform.position;
        timer = wanderTimer;
        lastPosition = transform.position;

        // Find player by tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;

        // Get animator if not assigned
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check movement for animator
        CheckMovement();

        // State machine
        switch (currentState)
        {
            case AIState.Idle:
                HandleIdleState(distanceToPlayer);
                break;
            case AIState.Wandering:
                HandleWanderingState(distanceToPlayer);
                break;
            case AIState.Following:
                HandleFollowingState(distanceToPlayer);
                break;
            case AIState.Attacking:
                HandleAttackingState(distanceToPlayer);
                break;
        }
    }

    void HandleIdleState(float distanceToPlayer)
    {
        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer <= attackRange)
            {
                ChangeState(AIState.Attacking);
            }
            else
            {
                ChangeState(AIState.Following);
            }
        }
        else
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                ChangeState(AIState.Wandering);
            }
        }
    }

    void HandleWanderingState(float distanceToPlayer)
    {
        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer <= attackRange)
            {
                ChangeState(AIState.Attacking);
            }
            else
            {
                ChangeState(AIState.Following);
            }
            return;
        }

        // Check if reached destination or stuck
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            ChangeState(AIState.Idle);
        }
    }

    void HandleFollowingState(float distanceToPlayer)
    {
        if (distanceToPlayer > detectionRange)
        {
            ChangeState(AIState.Idle);
        }
        else if (distanceToPlayer <= attackRange)
        {
            ChangeState(AIState.Attacking);
        }
        else
        {
            // Move towards player
            agent.SetDestination(player.position);
        }
    }

    void HandleAttackingState(float distanceToPlayer)
    {
        // Stop moving when attacking
        agent.ResetPath();

        if (distanceToPlayer > attackRange)
        {
            if (distanceToPlayer <= detectionRange)
            {
                ChangeState(AIState.Following);
            }
            else
            {
                ChangeState(AIState.Idle);
            }
        }
        else
        {
            // Attack if cooldown is ready
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                Attack();
            }
        }
    }

    void ChangeState(AIState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case AIState.Idle:
                agent.ResetPath();
                timer = wanderTimer;
                break;

            case AIState.Wandering:
                Vector3 newPos = RandomNavSphere(startPosition, wanderRadius);
                agent.SetDestination(newPos);
                timer = wanderTimer;
                break;

            case AIState.Following:
                // Destination will be set in HandleFollowingState
                break;

            case AIState.Attacking:
                agent.ResetPath();
                break;
        }
    }

    void Attack()
    {
        lastAttackTime = Time.time;

        // Face the player
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Keep on horizontal plane
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // Trigger attack animation
        if (animator != null)
        {
            animator.SetTrigger("attack");
        }

        // Add your attack logic here (damage, effects, etc.)
        Debug.Log("Enemy attacks player!");
    }

    void CheckMovement()
    {
        // Check if the enemy is moving
        float movementThreshold = 0.1f;
        Vector3 currentPosition = transform.position;
        bool wasMoving = isMoving;

        isMoving = Vector3.Distance(currentPosition, lastPosition) > movementThreshold;
        lastPosition = currentPosition;

        // Update animator walking parameter
        if (animator != null && wasMoving != isMoving)
        {
            animator.SetBool("walking", isMoving);
        }
    }

    Vector3 RandomNavSphere(Vector3 origin, float radius)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
        randomDirection += origin;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(randomDirection, out navHit, radius, -1))
        {
            return navHit.position;
        }

        return origin;
    }

    void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Draw wander area
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(startPosition, wanderRadius);
    }
}