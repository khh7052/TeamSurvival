using Constants;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NPCStateMachine))]
[RequireComponent(typeof(EntityModel))]
public class NPC : MonoBehaviour, IInteractable , IDeathBehavior
{
    [Header("Interact")]
    [SerializeField] private bool interactEnabled = true;
    [SerializeField] private string npcName;
    [SerializeField] private string interactPrompt = "대화하기 E";
    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private DialogueData adversarialDialogueData; // 적대적 상황일 때의 다이얼로그
    private bool isAdversarial = false;
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

    private GameObject homePointObj;

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

    [Header("Drop Item")]
    [SerializeField] private ItemData yieldItem;

    private float patrolDelay;
    private float lastPatrolTime;
    private float lastAttackTime;
    private NavMeshAgent agent;
    [SerializeField] private Transform nearestEnemyObject;
    private NPCStateMachine stateMachine;
    private AnimationHandler animationHandler;
    private EntityModel entityModel;

    // 읽기 전용 프로퍼티
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
        entityModel.OnHitEventWithgo += OnHit;
        stateMachine.Initialize(this);
        entityModel.health.OnChanged += Health_OnChanged;

        entityModel.moveSpeed.OnChangeValue += MoveSpeed_OnChanged;

        if(homePoint == null)
        {
            GameObject go = new GameObject("Home");
            go.transform.position = transform.position;
            homePoint = go.transform;
        }
    }

    private void Health_OnChanged()
    {
        if (IsDead)
        {
            if (entityModel.isDie) return;

            entityModel.isDie = true;   
            // NPC가 죽었을 때의 로직
            Debug.Log($"{npcName}이(가) 죽었습니다.");
            StopMoving();
            AnimationHandler?.PlayDie();
            Die();
        }
        else
        {
            // NPC가 데미지를 받았을 때의 로직
            // (현재 데미지 받았을 때의 이벤트가 따로 없으므로 일단 OnChanged에서 처리함, 나중에 수정필요)
            AnimationHandler?.PlayHit();
        }
    }

    public void Die()
    {
        entityModel.OnHitEventWithgo -= OnHit;
        DropLoot();
        StartCoroutine(FadeOutAndDestroy());
        WeatherCycle.Instance.RemoveObserver(entityModel);      
    }

    private IEnumerator FadeOutAndDestroy()
    {
        yield return new WaitForSeconds(5f);

        float duration = 2f;
        float elapsed = 0f;

        List<Material> materials = new List<Material>();
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            foreach (var mat in renderer.materials)
            {
                mat.SetFloat("_Mode", 2); 
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;

                materials.Add(mat);
            }
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

            foreach (var mat in materials)
            {
                if (mat.HasProperty("_Color"))
                {
                    Color color = mat.color;
                    color.a = alpha;
                    mat.color = color;
                }
            }

            yield return null;
        }

        gameObject.SetActive(false); //테스트
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
            Debug.Log($"{npcName}이(가) {nearestEnemyObject?.name}을(를) 공격합니다.");
            AnimationHandler?.PlayAttack();
            damageable.TakePhysicalDamage(attackDamage, gameObject);
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
        if (!isAdversarial)
        {
            DialogueManager.Instance.StartDialogue(dialogueData);
        }
        else
        {
            DialogueManager.Instance.StartDialogue(adversarialDialogueData);
        }
    }

    public string GetPrompt()
    {
        if (!interactEnabled) return "";
        
        return interactPrompt;
    }

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

    private void DropLoot()
    {
        Debug.Log($"[DropLoot] yieldItem: {yieldItem?.name}, ID: {yieldItem?.ID}");

        if (yieldItem == null) return;

        Vector3 pos = transform.position + Random.insideUnitSphere * 0.3f;
        if (pos.y < transform.position.y) pos.y = transform.position.y + 0.25f;

        AssetDataLoader.Instance.InstantiateByID(yieldItem.ID, (go) =>
        {
            if (go == null)
            {
                Debug.LogError($"[DropLoot] Failed to instantiate item with ID: {yieldItem.ID}");
                return;
            }

            go.transform.SetLocalPositionAndRotation(pos, transform.rotation);
            Debug.Log($"[DropLoot] Dropped item: {go.name} at {pos}");
        });
    }

    public void SetHome(Vector3 spawnPos)
    {
        homePointObj = new GameObject("HomePoint" + name);  
        homePoint = homePointObj.transform;
        homePoint.transform.position = spawnPos;
    }

    private void OnDisable()
    {
        if(homePointObj != null)
        {
            Destroy(homePointObj);
            homePointObj = null;
            homePoint = null;
        }
    }

    public void OnHit(GameObject attacker)
    {
        // 공격 받을 시 해당 레이어 공격 대상 추가
        int attackerLayer = attacker.gameObject.layer; 
        int attackerMask = 1 << attackerLayer;        

        targetLayerMask |= attackerMask;
        if (attacker.layer == LayerMask.NameToLayer("Player"))
        {
            interactEnabled = false;
            isAdversarial = true;
        }
    }
}
