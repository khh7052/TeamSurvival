using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    EntityModel model;

    public Vector2 curMovementInput;
    public LayerMask groundLayerMask;
    private EquipSystem equip;

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;
    private Vector2 mouseDelta;
    public bool canLook = true;

    private Rigidbody rb;

    [Header("Jump Stamina")]
    [SerializeField] private float jumpStaminaCost = 10f;

    AnimationHandler anim;
    public Action OnBuildModeInput;
    public BuildingMode buildMode;

    //[SerializeField]
    //private EntityModel condition;

    public bool IsDead => model.health.CurValue <= 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        model = GetComponent<EntityModel>();
        equip = GetComponent<EquipSystem>();
        anim = GetComponent<AnimationHandler>();
        model.OnDeathEvent += OnDeathEvent;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        if (IsDead) return;
        Move();
    }

    private void LateUpdate()
    {
        if(IsDead) return;
        if(canLook)
        {
            CameraLook();
        }
    }

    private void Move() //이동로직
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= model.moveSpeed.totalValue;
        dir.y = rb.velocity.y;

        rb.velocity = dir;
    }

    private void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    private void Jump() //점프로직
    {
        rb.AddForce(Vector2.up * model.jumpPower.totalValue, ForceMode.Impulse);
        anim.PlayerJump();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (IsDead) return;
        if(context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
            anim.PlayerWalk();
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
            anim.PlayerStop();
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (IsDead) return;
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (IsDead) return;
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            if (model != null && model.stamina != null)
                model.stamina.Subtract(jumpStaminaCost);

            Jump();
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (IsDead) return;
        if (context.phase == InputActionPhase.Started && equip != null && canLook)
        {
            equip.Attack();
        }
    }

    private bool IsGrounded() //땅에 있는지 체크해서 bool값으로 반환하는 함수
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down)
        };

        

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 1f, groundLayerMask))
            {
                Debug.DrawRay(rays[i].origin, rays[i].direction, Color.red);
                return true;
            }
        }
        return false;
    }

    public async void OnInventoryButton(InputAction.CallbackContext callbackContext)
    {
        if (IsDead) return;
        // 제작 켜져있을 땐 무시
        if (UIManager.Instance.IsEnableUI<CompositionUI>()) return;
        if (callbackContext.phase == InputActionPhase.Started)
        {
            if (UIManager.Instance.IsEnableUI<UIInventory>())
            {
                UIManager.Instance.CloseUI<UIInventory>();
            }
            else
            {
                await UIManager.Instance.ShowUI<UIInventory>();
            }
            ToggleCursor();
        }
    }

    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }


    public async void OnCraftUIButton(InputAction.CallbackContext callbackContext)
    {
        if (IsDead) return;
        // 인벤토리 켜져있을 땐 무시
        if (UIManager.Instance.IsEnableUI<UIInventory>()) return;
        if (callbackContext.phase == InputActionPhase.Started)
        {
            if (UIManager.Instance.IsEnableUI<CompositionUI>())
            {
                UIManager.Instance.CloseUI<CompositionUI>();
            }
            else
            {
                await UIManager.Instance.ShowUI<CompositionUI>();
            }
            ToggleCursor();
        }
    }

    public void OnBuildButton(InputAction.CallbackContext context)
    {
        if (IsDead) return;
        if (context.phase == InputActionPhase.Started)
        {
            buildMode.DestroyPrevObj();
            buildMode.isBuild = !buildMode.isBuild;
        }
    }

    public void OnBuildTryButton(InputAction.CallbackContext context)
    {
        if (IsDead) return;
        if (context.phase == InputActionPhase.Started)
        {
            if(buildMode.isBuild) 
                buildMode.TryBuild();
        }
    }

    public void OnDeathEvent()
    {
        // 일단 활성화되어있는 UI 닫기
        if (UIManager.Instance.IsEnableUI<CompositionUI>())
        {
            UIManager.Instance.CloseUI<CompositionUI>();
        }
        if (UIManager.Instance.IsEnableUI<UIInventory>())
        {
            UIManager.Instance.CloseUI<UIInventory>();
        }
        if (UIManager.Instance.IsEnableUI<InGameUI>())
        {
            UIManager.Instance.CloseUI<InGameUI>();
        }
        // 장착 무기도 없애기
        equip.UnEquip();
        GameManager.Instance.PlayerDead();
        StartCoroutine(DeathCameraEffect(transform));
    }

    public IEnumerator DeathCameraEffect(Transform player, float riseHeight = 5f, float duration = 2f)
    {
        Transform cam = Camera.main.transform;
        Vector3 startPos = cam.position;
        Vector3 targetPos = player.position + Vector3.up * riseHeight;
        int playerLayerMask = 1 << LayerMask.NameToLayer("Player");
        Camera.main.cullingMask |= playerLayerMask; 
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // 카메라 위치 보간 (위로 상승)
            cam.position = Vector3.Lerp(startPos, targetPos, t);

            // 카메라가 항상 플레이어 바라보게
            cam.LookAt(player.position);

            yield return null;
        }

        // 마지막 위치/각도 보정
        cam.position = targetPos;
        cam.LookAt(player.position);

        yield return new WaitForSeconds(1f);
        
        GameManager.Instance.LoadScene(SceneManager.GetActiveScene().name);
    }
}
