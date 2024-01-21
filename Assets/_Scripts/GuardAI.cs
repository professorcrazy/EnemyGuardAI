using UnityEngine;
using UnityEngine.AI;

public class GuardAI : MonoBehaviour
{

    private int currentPatrolIndex = 0;
    private Vector3 lastKnownPlayerPosition;
    private NavMeshAgent agent;
    private Transform player;
    [SerializeField] private Transform[] waypoints;

    private enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }

    private EnemyState currentState;
    [SerializeField] private float attackRange;
    [SerializeField] private float detectionRange;
    [SerializeField] private LayerMask playerLayer;
    void Start()
    {
        // ... (existing Start method code)
        agent = GetComponent<NavMeshAgent>();
        currentState = EnemyState.Patrol;
        SetDestination(waypoints[currentPatrolIndex].position);
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                CheckForPlayer();
                break;
            case EnemyState.Chase:
                Chase();
                CheckForPlayer();
                break;
            case EnemyState.Attack:
                Attack();
                break;
        }
    }

    void Patrol()
    {
        if (agent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % waypoints.Length;
            SetDestination(waypoints[currentPatrolIndex].position);
        }
    }

    void Chase()
    {
        SetDestination(player.position);

        if (Vector3.Distance(transform.position, player.position) < attackRange)
        {
            currentState = EnemyState.Attack;
        }
    }

    void Attack()
    {
        // Implement your attack logic here
        Debug.Log("Attacking the player");
    }

    void CheckForPlayer()
    {
        Collider[] playerTarget = Physics.OverlapSphere(transform.position, detectionRange, playerLayer);
        if (playerTarget.Length>0)
        {
            Transform tempTarget = null;
            float tempdist = float.MaxValue;
            for (int i = 0; i < playerTarget.Length; i++)
            {
                float curDist = Vector3.Distance(transform.position, playerTarget[i].transform.position);
                if (curDist < tempdist)
                {
                    tempdist = curDist;
                    tempTarget = playerTarget[i].transform;
                }
            }
            if (tempTarget != null)
            {
                player = tempTarget;
            }
        }
        else
        {
            SearchForPlayer();
        }
        if (Vector3.Distance(transform.position, player.position) < detectionRange)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, player.position - transform.position, out hit, detectionRange))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    currentState = EnemyState.Chase;
                    lastKnownPlayerPosition = player.position;
                }
            }
        }
    }

    void SetDestination(Vector3 target)
    {
        agent.SetDestination(target);
    }

    public void LoseSightOfPlayer()
    {
        SetDestination(lastKnownPlayerPosition);

        // If still can't see the player, try other positions
        SearchForPlayer();
    }

    public void ReturnToPatrol()
    {
        currentState = EnemyState.Patrol;
        SetDestination(waypoints[currentPatrolIndex].position);
    }

    void SearchForPlayer()
    {
        int positionsToTry = Random.Range(3, 6);

        for (int i = 0; i < positionsToTry; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(transform.position.x - 5f, transform.position.x + 5f),
                transform.position.y,
                Random.Range(transform.position.z - 5f, transform.position.z + 5f)
            );

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPosition, out hit, 2f, NavMesh.AllAreas))
            {
                SetDestination(hit.position);

                if (Vector3.Distance(transform.position, player.position) < detectionRange)
                {
                    RaycastHit playerHit;
                    if (Physics.Raycast(transform.position, player.position - transform.position, out playerHit, detectionRange, playerLayer))
                    {
                        if (playerHit.collider.CompareTag("Player"))
                        {
                            currentState = EnemyState.Chase;
                            lastKnownPlayerPosition = player.position;
                            return;
                        }
                    }
                }
            }
        }

        ReturnToPatrol();
    }
}
