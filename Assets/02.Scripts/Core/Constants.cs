using UnityEngine;

namespace Constants
{

    public enum SoundType
    {
        BGM,
        SFX,
    }

    public enum AIState
    {
        Idle, // 가만히 있기
        Flee, // 도망치기
        Attacking, // 공격하기
        Return // 귀환하기
    }


    public static class AnimatorHash
    {
        // Player
        public static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");


    }

}
