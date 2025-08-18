using UnityEngine;
using UnityEngine.InputSystem; // �� �Է� �ý���

public class DebugDamageOnKey : MonoBehaviour
{
    public int damage = 10;       // K�� ������ �� �� ������
    public Key key = Key.K;       // �׽�Ʈ Ű (�⺻ K)

    private EntityModel model;

    private void Awake()
    {
        model = GetComponent<EntityModel>();
        if (model == null)
            Debug.LogWarning("[DebugDamageOnKey] EntityModel�� �����ϴ�.", this);
    }

    private void Update()
    {
        if (model == null) return;

        var kb = Keyboard.current;
        if (kb == null) return;

        // K Ű�� ���� �� �����Ӹ� ����
        if (kb[key].wasPressedThisFrame)
        {
            model.TakePhysicalDamage(damage);
            Debug.Log($"[Debug] {key} ���� �� {damage} ������. " +
                      $"HP: {Mathf.RoundToInt(model.health.CurValue)}/{Mathf.RoundToInt(model.health.MaxValue)}",
                      this);
        }
    }
}
