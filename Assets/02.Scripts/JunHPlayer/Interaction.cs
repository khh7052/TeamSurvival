using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    [Header("Check Settings")]
    public float checkRate = 0.05f;
    private float lastCheckTime;
    public float maxCheckDistance = 3f;
    public LayerMask layerMask;       // "Interactable" ���� ���̾ ����

    [Header("Current Target")]
    public GameObject curInteractGameObject;
    private IInteractable curInteractable;     // IInteractable �������̽� ����

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
                // �� Ÿ������ �ٲ���� ���� ����
                if (hit.collider.gameObject != curInteractGameObject)
                {
                    curInteractGameObject = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                    SetPromptText();
                }
            }
            else
            {
                // �ƹ��͵� �� ���� ������ �ʱ�ȭ
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
        promptText.text = curInteractable.GetPrompt(); // �������� �����ϴ� �̸�/���� ǥ��
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        // Ű�� ���� ������ ó��
        if (context.phase == InputActionPhase.Started && curInteractable != null)
        {
            curInteractable.OnInteract(); // ������ ItemObject���� �α׸� ����
            curInteractGameObject = null;
            curInteractable = null;
            if (promptText != null) promptText.gameObject.SetActive(false);
        }
    }
}
