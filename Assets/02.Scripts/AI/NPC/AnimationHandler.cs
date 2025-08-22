using UnityEngine;
using Constants;

public class AnimationHandler : MonoBehaviour
{
    private Animator animator;

    private void Awake()
        => animator = GetComponent<Animator>();

    public void PlayIdle()
        => animator?.SetFloat(AnimatorHash.MoveSpeedHash, 0f);

    public void PlayWalk()
        => animator?.SetFloat(AnimatorHash.MoveSpeedHash, 0.5f);

    public void PlayRun()
        => animator?.SetFloat(AnimatorHash.MoveSpeedHash, 1f);

    public void PlayAttack()
        => animator?.SetTrigger(AnimatorHash.AttackHash);

    public void PlayHit()
        => animator?.SetTrigger(AnimatorHash.HitHash);

    public void PlayDie()
        => animator?.SetTrigger(AnimatorHash.DieHash);

    public void PlayerWalk() => animator?.SetBool(AnimatorHash.IsMove, true);
    public void PlayerStop() => animator?.SetBool(AnimatorHash.IsMove, false);
    public void PlayerJump() => animator?.SetTrigger(AnimatorHash.IsJump);

    public void OnFootstep()
    {
        Vector3 offset = new Vector3(0f, 0.1f, 0f); // 약간의 높이 조정
        FootstepManager.Instance.PlayFootstep(transform.position + offset);
    }

    public void OnJumpStart()
    {
    }
}
