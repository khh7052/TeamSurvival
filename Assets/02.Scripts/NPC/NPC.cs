using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Constants;

[RequireComponent(typeof(NavMeshAgent))]
public class NPC : MonoBehaviour, IInteractable
{
    [Header("Interact")]
    [SerializeField] private string interactPrompt = "��ȭ�ϱ� E";


    [Header("Stats")]
    [SerializeField] private int health;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;

    [Header("AI Settings")]
    private NavMeshAgent agent;
    private AIState aiState;
    [SerializeField] private Transform targetObject;
    private Vector3 targetPos;

    [SerializeField] private float detectDistance;

    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private LayerMask enemyLayerMask;
    private Transform nearestEnemyObject;
    [SerializeField] private float updateInterval = 0.2f; // �� Ž�� �ֱ�

    [Header("Flee")]
    [SerializeField] private float minFleeDistance;
    [SerializeField] private float maxFleeDistance;

    [Header("Return")]
    [SerializeField] private Transform homePoint; // ��ȯ ����

    private float playerDistance;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        StartCoroutine(UpdateNearestEnemyObject());
        SetState(AIState.Idle);
    }

    private void Update()
    {
        Action();
    }

    public void SetTargetPos(Vector3 newPos) => targetPos = newPos;

    public void SetState(AIState state)
    {
        if (aiState == state) return;

        aiState = state;

        agent.isStopped = aiState == AIState.Idle;

        switch (aiState)
        {
            case AIState.Idle:
                break;
            case AIState.Flee:
                FleeToNewLocation();
                break;
            case AIState.Attacking:
                //agent.SetDestination(targetPos);
                break;
            case AIState.Return:
                agent.SetDestination(homePoint.position);
                break;
        }
    }

    // ������Ʈ�ϸ鼭 ���ǰ˻��ϰ� ���º���
    public void Action()
    {
        playerDistance = Vector3.Distance(targetObject.position, transform.position);

        switch (aiState)
        {
            case AIState.Idle:
                IdleUpdate();
                break;
            case AIState.Flee:
                FleeUpdate();
                break;
            case AIState.Attacking:
                // AttackingUpdate();
                break;
            case AIState.Return:
                ReturnUpdate();
                break;
        }
    }

    void IdleUpdate()
    {
        // ���� ���� ���� ���� ������ ���� or ���ݻ��·� ��ȯ
        if(nearestEnemyObject != null)
        {
            SetState(AIState.Flee);
            return;
        }

        // ���� ���� ���� �÷��̾ ������ �ٶ󺸱�
    }

    void FleeUpdate()
    {
        // �ֺ��� ���� ������ ����ġ��
        if (nearestEnemyObject != null)
        {
            FleeToNewLocation();
        }
        // ���� ��, ����ģ ������ �̵��ϸ� ����
        else
        {
            if(agent.remainingDistance < 0.1f)
                SetState(AIState.Return);
        }
    }

    void ReturnUpdate()
    {
        // ��ȯ ���� �� �߰��ϸ� ����ġ��
        if (nearestEnemyObject != null)
        {
            SetState(AIState.Flee);
        }
        // ��ȯ ������ �����ϸ� Idle ���·� ����
        else
        {
            if (agent.remainingDistance < 0.1f)
                SetState(AIState.Return);
        }
    }


    void FleeToNewLocation()
    {
        SetState(AIState.Flee);
        agent.SetDestination(GetFleeLocation(nearestEnemyObject));
    }

    // �� Ž���ϱ�
    IEnumerator UpdateNearestEnemyObject()
    {
        while(true)
        {
            nearestEnemyObject = null;
            float nearestDistance = Mathf.Infinity;

            // �ֺ��� ���� �ִ��� Ȯ��
            Collider[] colliders = new Collider[5];
            if(Physics.OverlapSphereNonAlloc(transform.position, detectDistance, colliders, enemyLayerMask) > 0)
            {
                foreach (Collider collider in colliders)
                {
                    if (collider == null) break;

                    float distance = Vector3.Distance(collider.transform.position, transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestEnemyObject = collider.transform;
                    }
                }
            }

            yield return new WaitForSeconds(updateInterval);
        }
    }

    Vector3 GetFleeLocation(Transform nearestEnemyObject)
    {
        Vector3 fleeDirection = (transform.position - nearestEnemyObject.position).normalized;
        float fleeDistance = Random.Range(minFleeDistance, maxFleeDistance);
        Vector3 fleePosition = transform.position + fleeDirection * fleeDistance;

        if (NavMesh.SamplePosition(fleePosition, out NavMeshHit hit, maxFleeDistance, NavMesh.AllAreas))
            return hit.position;
        else
        {
            int i = 0;
            while (i < 10) // �ִ� 10�� �õ�
            {
                fleeDistance = Random.Range(minFleeDistance, maxFleeDistance);
                fleePosition = transform.position + fleeDirection * fleeDistance;

                if (NavMesh.SamplePosition(fleePosition, out hit, maxFleeDistance, NavMesh.AllAreas))
                    return hit.position;

                i++;
            }
        }

        return transform.position; // Fallback to current position if no valid position found
    }


    public void OnInteract()
    {
        
    }

    public string GetPrompt()
    {
        return interactPrompt;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectDistance);
        if (nearestEnemyObject != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, nearestEnemyObject.position);
        }
    }


}
