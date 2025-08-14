using System.Collections.Generic;
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

    public enum ePlayerState
    {
        Idle, Move, Run, // Add need more
    }

    public enum eEnemyState
    {
        Idle, Wathering, Run, // Add need more
    }


    public static class UIPrefabPath
    {
        public static Dictionary<string, string> paths = new()
        {
            { "Canvas", "UI/Canvas" },
            { "TestConditionUI" , "UI/ConditionUI" }
        };
        public static string GetPrefabPath(string prefabName)
        {
            if(paths.ContainsKey(prefabName)) 
            {
                return paths[prefabName];
            }
            return "";
        }
    }
}
