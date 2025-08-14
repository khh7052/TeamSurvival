using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Constants;

[RequireComponent(typeof(NavMeshAgent))]
public class NPC : MonoBehaviour, IInteractable
{
    [Header("Interact")]
    [SerializeField] private string npcName;
    [SerializeField] private string interactPrompt = "��ȭ�ϱ� E";
    [SerializeField] private DialogueData dialogueData; // ��ȭ ������
    private NPCView npcView; // NPC �� ������Ʈ

    [Header("Stats")]
    [SerializeField] private int health;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float lookSpeed;

    [Header("AI Settings")]
    private NavMeshAgent agent;
    private AIState aiState;
    [SerializeField] private Transform targetObject;
    private Vector3 targetPos;

    [SerializeField] private float detectDistance;
    [SerializeField] private LayerMask enemyLayerMask;
    private Transform nearestEnemyObject;
    [SerializeField] private float updateInterval = 0.2f; // �� Ž�� �ֱ�
    private float playerDistance;

    [Header("Flee")]
    [SerializeField] private float minFleeDistance;
    [SerializeField] private float maxFleeDistance;

    [Header("Return")]
    [SerializeField] private Transform homePoint; // ��ȯ ����



    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        npcView = GetComponent<NPCView>();
    }

    private void Start()
    {
        npcView.SetName(npcName);
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

        switch (aiState)
        {
            case AIState.Idle:
                targetPos = transform.position;
                break;
            case AIState.Flee:
                targetPos = GetFleeLocation(nearestEnemyObject);
                break;
            case AIState.Attack:

                break;
            case AIState.Return:
                targetPos = homePoint.position;
                break;
        }

        agent.isStopped = aiState == AIState.Idle;
        agent.SetDestination(targetPos);
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
            case AIState.Attack:
                // AttackingUpdate();
                break;
            case AIState.Return:
                ReturnUpdate();
                break;
        }

        agent.SetDestination(targetPos);
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
        if(playerDistance <= detectDistance)
        {
            LookTarget(targetObject);
        }
    }

    void FleeUpdate()
    {
        // �ֺ��� ���� ������ ����ġ��
        if (nearestEnemyObject != null)
        {
            targetPos = GetFleeLocation(nearestEnemyObject);
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

    // NPC�� �ٶ󺸱�
    void LookTarget(Transform target)
    {
        if (target == null) return;

        Vector3 direction = target.position - transform.position;
        direction.y = 0;
        direction.Normalize();

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        if (Quaternion.Angle(transform.rotation, lookRotation) >= 1f)
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookSpeed);
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


    [ContextMenu("Interact")]
    public void OnInteract()
    {
        DialogueManager.Instance.StartDialogue(dialogueData, () => Debug.Log("�ݹ� �׽�Ʈ"));
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
