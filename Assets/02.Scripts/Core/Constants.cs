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
        Idle, // ������ �ֱ�
        Flee, // ����ġ��
        Attacking, // �����ϱ�
        Return // ��ȯ�ϱ�
    }


    public static class AnimatorHash
    {
        // Player
        public static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");


    }

}
