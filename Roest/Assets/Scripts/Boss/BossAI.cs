using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class BossAI : NetworkBehaviour
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
    [SyncVar(hook = nameof(OnHealthChanged))]
    public int currentHealth;
    [SerializeField]
    public float patrolSpeed = 3f;
    [SerializeField]
    public float chaseSpeed = 5f;
    private Transform player;

    private bool hasDestination;
    private Vector3 chaseDirection;
    private bool isAttacking;
    private bool isAlive = true;
    private bool hasDied = false;
    private bool aggro = false;

    [Header("Boss Combat Patterns")]
    public float patternSwitchDelay = 3f;
    private bool isSwitchingPattern = false;
    private IEnumerator CurrentPattern;
    public Transform flamethrowerSpawnPoint;
    public GameObject flamethrowerPrefab;
    public GameObject flameClawPrefab;
    public GameObject flameHornPrefab;
    public GameObject fireworksPrefab;
    private GameObject jawGameObject;
    [SyncVar]
    private int randomPattern;

    void Start()
    {
        if (!isAlive) return;
        navMeshAgent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;
        Transform root = transform.Find("Root");
        if (root != null)
        {
            jawGameObject = root.Find("Jaw").gameObject;
        }

        if (isServer)
        {
            // Seul le serveur génère le modèle aléatoire et le synchronise
            StartCoroutine(GenerateAndSyncRandomPattern());
        }
    }

    IEnumerator GenerateAndSyncRandomPattern()
    {
        while (true)
        {
            int newRandomPattern = Random.Range(0, 3);
            randomPattern = newRandomPattern;

            // Inform clients about the change
            RpcSyncRandomPattern(randomPattern);

            // Attendez avant de générer un nouveau modèle aléatoire
            yield return new WaitForSeconds(patternSwitchDelay);
        }
    }

    IEnumerator SyncRandomPattern()
    {
        // Wait for the network to be ready
        yield return new WaitUntil(() => isServer);

        // Synchronize the randomPattern across the network initially
        CmdSyncRandomPattern(Random.Range(0, 3));
    }

    [Command]
    void CmdSyncRandomPattern(int newPattern)
    {
        randomPattern = newPattern;

        // Inform clients about the change
        RpcSyncRandomPattern(randomPattern);
    }

    [ClientRpc]
    void RpcSyncRandomPattern(int newPattern)
    {
        // Update the randomPattern on clients
        randomPattern = newPattern;
    }

    void Update()
    {
        if (!isAlive) return;
        if (aggro) {
            StartCoroutine(SyncRandomPattern());
            float detectionDistanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (detectionDistanceToPlayer < detectionDistance)
            {
                if(!isAttacking)
                {
                    Quaternion rot = Quaternion.LookRotation(player.position - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
                    if (Vector3.Distance(player.position, transform.position) < attackRadius)
                    {
                        StartCoroutine(AttackPlayer());
                    }
                    else
                    {
                        navMeshAgent.isStopped = false;
                        navMeshAgent.speed = chaseSpeed;
                        navMeshAgent.SetDestination(player.position);
                    }
                }
            }
            else if (detectionDistance < detectionDistanceToPlayer && detectionDistanceToPlayer < detectionDistance + 50)
            {
                
                StartCoroutine(SwitchCombatPattern());
                if (!isAttacking)
                {
                    Chase();
                } else {
                    StopChasing();
                    navMeshAgent.isStopped = true;
                    navMeshAgent.speed = 0;
                }
            } else {
                StopChasing();
            }
        }
        if(navMeshAgent.isOnNavMesh && navMeshAgent.remainingDistance < 0.75f && !hasDestination && !aggro)
        {
            StartCoroutine(GetNewDestination());
        }
        if (animator){
            animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
        }
        if (currentHealth <= 0 && !hasDied)
        {
            isAlive = false;
            hasDied = true;

            Die();
        }
        StartCoroutine(SyncRandomPattern());
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
        jawGameObject.SetActive(true);

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackDelay);

        if(navMeshAgent.enabled)
        {
            navMeshAgent.isStopped = false;
        }
        jawGameObject.SetActive(false);
        isAttacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            aggro = true;
            Vector3 lookDirection = player.position - transform.position;
            lookDirection.y = 0f;
            Quaternion rotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }
    }

    void Chase()
    {
        if (!isAlive) return;
        chaseDirection = player.position - transform.position;
        chaseDirection.y = 0;
        Vector3 chasePosition = transform.position + chaseDirection.normalized * detectionDistance;

        navMeshAgent.destination = chasePosition;
        navMeshAgent.speed = chaseSpeed;
    }

    void StopChasing()
    {
        navMeshAgent.speed = patrolSpeed;
        Debug.Log("STOP CHASE");
    }

    void Die()
    {
        if (!isAlive) return;

        isAlive = false;
        navMeshAgent.isStopped = true;
        chaseSpeed = 0f;
        patrolSpeed = 0f;

        animator.SetBool("Die", true);

        StartCoroutine(DisableGameObjectAfterDeath());
    }

    private IEnumerator DisableGameObjectAfterDeath()
    {
        hasDied = true;
        yield return new WaitForSeconds(2);

        navMeshAgent.enabled = false;
        Instantiate(fireworksPrefab, flamethrowerSpawnPoint.position, flamethrowerSpawnPoint.rotation);
        yield return new WaitForSeconds(20);
        OnCoroutineEnd();
    }

    private void OnCoroutineEnd()
    {
        NetworkServer.Destroy(gameObject);
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void TakeDamage(int damage)
    {
        if (!isServer) return;

        if (isAlive)
        {
            currentHealth -= damage;
            Debug.Log("Dragon: Damage done: " + damage + " Current health: " + currentHealth);
        }
    }

    void OnHealthChanged(int oldValue, int newValue)
    {
        Debug.Log("Health changed from " + oldValue + " to " + newValue);
        if (newValue <= 0 && !hasDied)
        {
            Die();
        }
    }

    IEnumerator SwitchCombatPattern()
    {
        if (isSwitchingPattern)
            yield break;

        isSwitchingPattern = true;

        yield return new WaitForSeconds(patternSwitchDelay);

        switch (randomPattern)
        {
            case 0:
                CurrentPattern = BossPattern1();
                break;
            case 1:
                CurrentPattern = BossPattern2();
                break;
            case 2:
                CurrentPattern = BossPattern3();
                break;
        }

        yield return StartCoroutine(CurrentPattern);  // Attendre la fin de la coroutine du modèle actuel

        isSwitchingPattern = false;
    }

    IEnumerator BossPattern1()
    {
        RotateTowardsPlayer();
        animator.SetTrigger("Claw");

        yield return new WaitForSeconds(2);
        
        Instantiate(flameClawPrefab, flamethrowerSpawnPoint.position, flamethrowerSpawnPoint.rotation);
        navMeshAgent.isStopped = true;

        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);

        isSwitchingPattern = false;
    }

    IEnumerator BossPattern2()
    {
        RotateTowardsPlayer();
        animator.SetTrigger("Horn");

        yield return new WaitForSeconds(1);
        
        Instantiate(flameHornPrefab, flamethrowerSpawnPoint.position, flamethrowerSpawnPoint.rotation);
        navMeshAgent.isStopped = true;

        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);

        isSwitchingPattern = false;
    }

    IEnumerator BossPattern3()
    {
        RotateTowardsPlayer();
        animator.SetTrigger("Scream");

        yield return new WaitForSeconds(1);
        
        Instantiate(flamethrowerPrefab, flamethrowerSpawnPoint.position, flamethrowerSpawnPoint.rotation);
        navMeshAgent.isStopped = true;

        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);

        isSwitchingPattern = false;
    }

    void RotateTowardsPlayer()
    {
        if (player != null)
        {
            Vector3 lookDirection = player.position - transform.position;
            lookDirection.y = 0f;
            Quaternion rotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }
    }
}