using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    [Header("Check Settings")]
    public float checkRate = 0.05f;
    private float lastCheckTime;
    public float maxCheckDistance = 3f;
    public LayerMask layerMask;       // "Interactable" 같은 레이어를 지정

    [Header("Current Target")]
    public GameObject curInteractGameObject;
    private IInteractable curInteractable;     // IInteractable 인터페이스 참조

    [Header("UI")]
    public TextMeshProUGUI promptText;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        if (promptText != null) promptText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                // 새 타겟으로 바뀌었을 때만 갱신
                if (hit.collider.gameObject != curInteractGameObject)
                {
                    curInteractGameObject = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                    SetPromptText();
                }
            }
            else
            {
                // 아무것도 안 보고 있으면 초기화
                curInteractGameObject = null;
                curInteractable = null;
                if (promptText != null) promptText.gameObject.SetActive(false);
            }
        }
    }

    private void SetPromptText()
    {
        if (promptText == null || curInteractable == null) return;

        promptText.gameObject.SetActive(true);
        promptText.text = curInteractable.GetPrompt(); // 아이템이 제공하는 이름/설명 표시
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        // 키가 눌린 순간만 처리
        if (context.phase == InputActionPhase.Started && curInteractable != null)
        {
            curInteractable.OnInteract(); // 아직은 ItemObject에서 로그만 찍음
            curInteractGameObject = null;
            curInteractable = null;
            if (promptText != null) promptText.gameObject.SetActive(false);
        }
    }
}
