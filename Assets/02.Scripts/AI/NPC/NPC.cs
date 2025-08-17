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
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private float updateInterval = 0.2f;
    [SerializeField] private Transform playerObject;

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
    private AnimationHandler animationHandler;
    private EntityModel entityModel;

    // 읽기 전용 프로퍼티
    public bool CanAttack => attackEnabled;
    public float DetectDistance => detectDistance;
    public float AttackDistance => attackDistance;
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
    }

    private void Health_OnChanged()
    {
        if (IsDead)
        {
            // NPC가 죽었을 때의 로직
            Debug.Log($"{npcName}이(가) 죽었습니다.");
            StopMoving();
            AnimationHandler?.PlayDie();
        }
        else
        {
            // NPC가 데미지를 받았을 때의 로직
            // (현재 데미지 받았을 때의 이벤트가 따로 없으므로 일단 OnChanged에서 처리함, 나중에 수정필요)
            AnimationHandler?.PlayHit();
        }
    }

    private void Start()
    {
        npcView?.SetName(npcName);
        StartCoroutine(UpdateNearestEnemyObject());
        SetSpeed(moveSpeed);
        stateMachine.ChangeState<NPCIdleState>();
    }

    private void Update()
    {
        if(IsDead) return;

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

        if (Time.time >= lastAttackTime + attackDelay)
        {
            lastAttackTime = Time.time;
            // 공격 로직 구현
            IDamageable damageable = nearestEnemyObject.GetComponentInParent<IDamageable>();

            if (damageable != null)
            {
                Debug.Log($"{npcName}이(가) {nearestEnemyObject?.name}을(를) 공격합니다.");
                AnimationHandler?.PlayAttack();
                damageable.TakePhysicalDamage(attackDamage);
            }
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
