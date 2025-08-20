using UnityEngine;
using UnityEngine.InputSystem; // 새 입력 시스템

public class DebugDamageOnKey : MonoBehaviour
{
    public int damage = 10;       // K를 눌렀을 때 줄 데미지
    public Key key = Key.K;       // 테스트 키 (기본 K)

    private EntityModel model;

    private void Awake()
    {
        model = GetComponent<EntityModel>();
        if (model == null)
            Debug.LogWarning("[DebugDamageOnKey] EntityModel이 없습니다.", this);
    }

    private void Update()
    {
        if (model == null) return;

        var kb = Keyboard.current;
        if (kb == null) return;

        // K 키가 눌린 한 프레임만 감지
        if (kb[key].wasPressedThisFrame)
        {
            model.TakePhysicalDamage(damage);
            Debug.Log($"[Debug] {key} 눌림 → {damage} 데미지. " +
                      $"HP: {Mathf.RoundToInt(model.health.CurValue)}/{Mathf.RoundToInt(model.health.MaxValue)}",
                      this);
        }
    }
}
