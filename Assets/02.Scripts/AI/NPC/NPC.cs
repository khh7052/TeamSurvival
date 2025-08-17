using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Constants;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NPCStateMachine))]
public class NPC : MonoBehaviour, IInteractable
{
    [Header("Interact")]
    [SerializeField] private string npcName;
    [SerializeField] private string interactPrompt = "대화하기 E";
    [SerializeField] private DialogueData dialogueData;
    private NPCView npcView;

    [Header("Stats")]
    [SerializeField] private int health;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float lookSpeed;

    [Header("AI Settings")]
    [SerializeField] private bool attackEnabled = true;
    [SerializeField] private float detectDistance;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private float updateInterval = 0.2f;
    [SerializeField] private Transform targetObject;

    [Header("Flee")]
    [SerializeField] private float minFleeDistance;
    [SerializeField] private float maxFleeDistance;

    [Header("Return")]
    [SerializeField] private Transform homePoint;
    [SerializeField] private float limitMoveRange = 10f;

    [Header("Attack")]
    [SerializeField] private float attackDistance = 5f;
    [SerializeField] private float attackDelay = 1f;
    [SerializeField] private int attackDamage = 1;

    private float lastAttackTime;
    private NavMeshAgent agent;
    private Transform nearestEnemyObject;
    private NPCStateMachine stateMachine;

    // 읽기 전용 프로퍼티
    public bool CanAttack => attackEnabled;
    public float DetectDistance => detectDistance;
    public float AttackDistance => attackDistance;
    public Transform NearestEnemy => nearestEnemyObject;
    public Transform Target => targetObject;
    public Transform HomePoint => homePoint;
    public float PlayerDistance => targetObject ? Vector3.Distance(targetObject.position, transform.position) : float.MaxValue;
    public NavMeshAgent Agent => agent;
    public float LookSpeed => lookSpeed;
    public float LimitMoveRange => limitMoveRange;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        npcView = GetComponent<NPCView>();
        stateMachine = GetComponent<NPCStateMachine>();
        stateMachine.Initialize(this);
    }

    private void Start()
    {
        npcView.SetName(npcName);
        StartCoroutine(UpdateNearestEnemyObject());
        SetSpeed(moveSpeed);
        stateMachine.ChangeState<NPCIdleState>();
    }

    private void Update()
    {
        stateMachine.UpdateState();
    }

    public void SetSpeed(float speed) => agent.speed = speed;

    public void MoveTo(Vector3 position)
    {
        agent.isStopped = false;
        agent.SetDestination(position);
    }

    public void StopMoving() => agent.isStopped = true;

    public void TryAttack()
    {
        if (Time.time >= lastAttackTime + attackDelay)
        {
            lastAttackTime = Time.time;
            Debug.Log($"{npcName}이(가) {nearestEnemyObject?.name}을(를) 공격합니다.");
        }
    }

    public Vector3 GetFleeLocation()
        => transform.GetFleePositionFrom(nearestEnemyObject, minFleeDistance, maxFleeDistance);

    private IEnumerator UpdateNearestEnemyObject()
    {
        while (true)
        {
            nearestEnemyObject = null;
            float nearestDist = Mathf.Infinity;

            Collider[] cols = new Collider[5];
            if (Physics.OverlapSphereNonAlloc(transform.position, detectDistance, cols, enemyLayerMask) > 0)
            {
                foreach (var col in cols)
                {
                    if (col == null) break;
                    float dist = Vector3.Distance(col.transform.position, transform.position);
                    if (dist < nearestDist)
                    {
                        nearestDist = dist;
                        nearestEnemyObject = col.transform;
                    }
                }
            }
            yield return new WaitForSeconds(updateInterval);
        }
    }

    [ContextMenu("Interact")]
    public void OnInteract()
    {
        DialogueManager.Instance.StartDialogue(dialogueData, () => Debug.Log("콜백 테스트"));
    }

    public string GetPrompt() => interactPrompt;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        if (nearestEnemyObject != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, nearestEnemyObject.position);
        }
        if (homePoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(homePoint.position, limitMoveRange);
        }
    }
}
