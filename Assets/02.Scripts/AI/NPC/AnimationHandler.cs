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
}
