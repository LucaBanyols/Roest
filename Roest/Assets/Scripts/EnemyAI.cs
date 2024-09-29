using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class EnemyAI : NetworkBehaviour
{
    [Header("References")]
    [SerializeField]
    private NavMeshAgent navMeshAgent;
    [SerializeField]
    private Animator animator;
    public float detectionDistance = 5f;
    [SerializeField]
    private float attackRadius = 1f;
    [SerializeField]
    private float attackDelay = 2f;
    [SerializeField]
    private float rotationSpeed;

    [Header("Wandering parameters")]
    [SerializeField]
    private float wanderingWaitTimeMin;
    [SerializeField]
    private float wanderingWaitTimeMax;
    [SerializeField]
    private float wanderingDistanceMin;
    [SerializeField]
    private float wanderingDistanceMax;

    [Header("Stats")]
    [SerializeField]
    private int maxHealth = 100;
    [SerializeField]

    [SyncVar(hook = nameof(OnHealthChanged))]
    public int currentHealth;

    [SerializeField]
    public float patrolSpeed = 3f;
    [SerializeField]
    public float chaseSpeed = 5f;
    private Transform player;
    public bool chaseType = false;

    private bool hasDestination;
    private bool fleeing = false;
    private Vector3 fleeDirection;
    private bool isAttacking;
    private bool isAlive = true;
    private bool hasDied = false;
    private bool aggro = false;
    private GameObject hornGameObject;

    void Start()
    {
        if (!isAlive) return;

        navMeshAgent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;
        if (chaseType) {
            hornGameObject = transform.Find("Horn").gameObject;
        }   
    }

    void Update()
    {
        if (!isAlive) return;

        if (aggro && player) {
            float detectionDistanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (detectionDistanceToPlayer < detectionDistance)
            {
                if(!isAttacking && chaseType == true)
                {
                    Quaternion rot = Quaternion.LookRotation(player.position - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
                    if (Vector3.Distance(player.position, transform.position) < attackRadius)
                    {
                        StartCoroutine(AttackPlayer());
                    }
                    else
                    {
                        navMeshAgent.SetDestination(player.position);
                    }
                }
                Flee();
            }
            else if (fleeing)
            {
                StopFleeing();
            }
        }
        if(navMeshAgent.isOnNavMesh && navMeshAgent.remainingDistance < 0.75f && !hasDestination)
        {
            StartCoroutine(GetNewDestination());
        }
        if (animator){
            animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
        }
    }

    IEnumerator GetNewDestination()
    {
        hasDestination = true;
        yield return new WaitForSeconds(Random.Range(wanderingWaitTimeMin, wanderingWaitTimeMax));

        Vector3 nextDestination = transform.position;
        nextDestination += Random.Range(wanderingDistanceMin, wanderingDistanceMax) * new Vector3(Random.Range(-1f, 1), 0f, Random.Range(-1f, 1f)).normalized;

        NavMeshHit hit;
        if(NavMesh.SamplePosition(nextDestination, out hit, wanderingDistanceMax, NavMesh.AllAreas))
        {
            navMeshAgent.SetDestination(hit.position);
        }
        hasDestination = false;
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;
        navMeshAgent.isStopped = true;
        hornGameObject.SetActive(true);

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackDelay);

        if(navMeshAgent.enabled)
        {
            navMeshAgent.isStopped = false;
        }
        hornGameObject.SetActive(false);
        isAttacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            aggro = true;
        }
    }

    void Flee()
    {
        if (!isAlive) return;

        if (chaseType == false) {
            fleeDirection = transform.position - player.position;
        } else {
            fleeDirection = player.position - transform.position;
        }
        fleeDirection.y = 0;
        Vector3 fleePosition = transform.position + fleeDirection.normalized * detectionDistance;
        navMeshAgent.destination = fleePosition;
        navMeshAgent.speed = chaseSpeed;
        fleeing = true;
    }

    void StopFleeing()
    {
        navMeshAgent.speed = patrolSpeed;
        fleeing = false;
    }

    public void TakeDamage(int damage)
    {
        if (!isServer) return;

        if (isAlive)
        {
            currentHealth -= damage;
            Debug.Log(chaseType ? "GOAT " : "SHEEP " + "Damage done: " + damage + " Current health: " + currentHealth);
        }
    }

    // [Command]
    // void CmdDeath()
    // {
    //     DeathRPC();
    // }

    // [ClientRpc]
    // void DeathRPC()
    // {
    //     Die();
    // }

    void Die()
    {
        if (!isAlive) return;

        isAlive = false;
        navMeshAgent.isStopped = true;
        chaseSpeed = 0f;
        patrolSpeed = 0f;

        animator.SetBool("isDead", true);

        StartCoroutine(DisableGameObjectAfterDeath());
    }

    private IEnumerator DisableGameObjectAfterDeath()
    {
        hasDied = true;
        yield return new WaitForSeconds(5);

        navMeshAgent.enabled = false;
        NetworkServer.Destroy(gameObject);
    }

    void OnHealthChanged(int oldValue, int newValue)
    {
        Debug.Log("Health changed from " + oldValue + " to " + newValue);
        if (newValue <= 0 && !hasDied)
        {
            Die();
        }
    }
}