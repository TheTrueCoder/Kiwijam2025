using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;


public class HorrorAIEnemy : MonoBehaviour
{
    const string attackAnimationTrigger = "attack";
    const string walkingOrIdleAnimationBoolean = "walking";

    public NavMeshAgent agent;
    public Animator enemyCharAnimator;
    public UnityEvent attackAnimationEvent;

    private Transform playerTransform;

    public LayerMask whatIsGround, whatIsPlayer;

    public string playerTag = "Player";

    public float groundSearchDistance = 2.0f;
    private float distanceThresholdForIdleMovement = 1f; 

    //Idle
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks = 5f;
    bool alreadyAttacked = false;

    //States
    public float sightRange = 10f;
    public float attackRange = 1f;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag(playerTag).transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Idle();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
    }
    private void SeachWalkPoint()
    {
        //Random patrol walking direction
        float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsPlayer))
        {
            walkPointSet = true;
        }
    }
    private void Idle()
    {
        if (!walkPointSet) SeachWalkPoint();

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        //When reached
        if (distanceToWalkPoint.magnitude > distanceThresholdForIdleMovement)
        {
            walkPointSet = false;
        }
    }
    
    private void ChasePlayer()
    {
        agent.SetDestination(transform.position);
    }
    private void AttackPlayer()
    {
        //Stop Enemey and face at player
        agent.SetDestination(transform.position);
        transform.LookAt(playerTransform);

        if (!alreadyAttacked)
        {
            //Attack code
            enemyCharAnimator.SetTrigger(attackAnimationTrigger);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
}