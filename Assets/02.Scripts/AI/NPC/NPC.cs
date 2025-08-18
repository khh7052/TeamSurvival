using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Constants;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NPCStateMachine))]
[RequireComponent(typeof(EntityModel))]
public class NPC : MonoBehaviour, IInteractable
{
    [Header("Interact")]
    [SerializeField] private bool interactEnabled = true;
    [SerializeField] private string npcName;
    [SerializeField] private string interactPrompt = "��ȭ�ϱ� E";
    [SerializeField] private DialogueData dialogueData;
    private NPCView npcView;

    [Header("Stats")]
//    [SerializeField] private float moveSpeed;
    [SerializeField] private float lookSpeed;

    [Header("AI Settings")]
    [SerializeField] private float detectDistance;
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private float updateInterval = 0.2f;
    [SerializeField] private Transform playerObject;

    [Header("Flee")]
    [SerializeField] private float minFleeDistance;
    [SerializeField] private float maxFleeDistance;

    [Header("Return")]
    [SerializeField] private Transform homePoint;
    [SerializeField] private float limitMoveRange = 10f;

    [Header("Patrol")]
    [SerializeField] private bool patrolEnabled = true;
    [SerializeField] private float patrolMinDistance = 3f;
    [SerializeField] private float patrolMaxDistance = 5f;
    [SerializeField] private float patrolMinDelay = 5f;
    [SerializeField] private float patrolMaxDelay = 10f;

    [Header("Attack")]
    [SerializeField] private bool attackEnabled = true;
    [SerializeField] private float attackDistance = 5f;
    [SerializeField] private float attackDelay = 1f;
    [SerializeField] private int attackDamage = 1;

    [Header("Debug")]
    [SerializeField] private bool useGizmo = true;

    private float patrolDelay;
    private float lastPatrolTime;
    private float lastAttackTime;
    private NavMeshAgent agent;
    [SerializeField] private Transform nearestEnemyObject;
    private NPCStateMachine stateMachine;
    private AnimationHandler animationHandler;
    private EntityModel entityModel;

    // �б� ���� ������Ƽ
    public bool CanPatrol => patrolEnabled;
    public bool CanAttack => attackEnabled;
    public float DetectDistance => detectDistance;
    public float AttackDistance => attackDistance;
    public float RemainingDistance => agent.remainingDistance;
    public Transform NearestEnemy => nearestEnemyObject;
    public Transform Player => playerObject;
    public Transform HomePoint => homePoint;
    public float PlayerDistance => playerObject ? Vector3.Distance(playerObject.position, transform.position) : float.MaxValue;
    public NavMeshAgent Agent => agent;
    public float LookSpeed => lookSpeed;
    public float LimitMoveRange => limitMoveRange;

    public bool IsDead => entityModel.health.CurValue <= 0;

    public AnimationHandler AnimationHandler => animationHandler;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        npcView = GetComponent<NPCView>();
        stateMachine = GetComponent<NPCStateMachine>();
        animationHandler = GetComponentInChildren<AnimationHandler>();
        entityModel = GetComponent<EntityModel>();

        stateMachine.Initialize(this);
        entityModel.health.OnChanged += Health_OnChanged;

        entityModel.moveSpeed.OnChangeValue += MoveSpeed_OnChanged;
    }

    private void Health_OnChanged()
    {
        if (IsDead)
        {
            // NPC�� �׾��� ���� ����
            Debug.Log($"{npcName}��(��) �׾����ϴ�.");
            StopMoving();
            AnimationHandler?.PlayDie();
        }
        else
        {
            // NPC�� �������� �޾��� ���� ����
            // (���� ������ �޾��� ���� �̺�Ʈ�� ���� �����Ƿ� �ϴ� OnChanged���� ó����, ���߿� �����ʿ�)
            AnimationHandler?.PlayHit();
        }
    }

    private void MoveSpeed_OnChanged()
    {
        SetSpeed(entityModel.moveSpeed.totalValue);
    }

    private void Start()
    {
        npcView?.SetName(npcName);
        StartCoroutine(UpdateNearestEnemyObject());
        SetSpeed(entityModel.moveSpeed.totalValue);
        stateMachine.ChangeState<NPCIdleState>();
    }

    private void Update()
    {
        if (IsDead) return;

        stateMachine.UpdateState();
    }

    public void SetSpeed(float speed) => agent.speed = speed;

    public void MoveTo(Vector3 position)
    {
        AnimationHandler?.PlayRun();
        agent.isStopped = false;
        agent.SetDestination(position);
    }

    public void StopMoving()
    {
        AnimationHandler?.PlayIdle();
        agent.isStopped = true;
    }

    public void TryAttack()
    {
        if (nearestEnemyObject == null) return;
        if (Time.time < lastAttackTime + attackDelay) return;

        lastAttackTime = Time.time;
        IDamageable damageable = nearestEnemyObject.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            Debug.Log($"{npcName}��(��) {nearestEnemyObject?.name}��(��) �����մϴ�.");
            AnimationHandler?.PlayAttack();
            damageable.TakePhysicalDamage(attackDamage);
        }
    }


    public void TryPatrol()
    {
        if (Time.time < lastPatrolTime + patrolDelay) return;
        lastPatrolTime = Time.time;
        patrolDelay = Random.Range(patrolMinDelay, patrolMaxDelay);
        Vector3 randomPos = transform.GetRandomPosition(patrolMinDistance, patrolMaxDistance);
        Vector3 desination = randomPos.ClampDistanceFrom(HomePoint.position, LimitMoveRange);
        MoveTo(desination);
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
            if (Physics.OverlapSphereNonAlloc(transform.position, detectDistance, cols, targetLayerMask) > 0)
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
        if (!interactEnabled) return;
        DialogueManager.Instance.StartDialogue(dialogueData);
    }

    public string GetPrompt() => interactPrompt;

    private void OnDrawGizmos()
    {
        if(!useGizmo) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        if (agent != null && !agent.isStopped)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, agent.destination);
        }

        if (homePoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(homePoint.position, limitMoveRange);
        }
    }
}
