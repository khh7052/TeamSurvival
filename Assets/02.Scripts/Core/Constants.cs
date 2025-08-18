using System.Collections.Generic;
using UnityEngine;

namespace Constants
{

    public enum SoundType
    {
        BGM,
        SFX,
    }

    public enum VolumeType
    {
        Master,
        BGM,
        SFX
    }

    public static class AudioConstants
    {
        public static readonly float MasterVolume = 1f;
        public static readonly float BGMVolume = 0.5f;
        public static readonly float SFXVolume = 0.5f;

        public static readonly float MinVolume = 0.001f;
        public static readonly float MaxVolume = 1f;

        public static Dictionary<VolumeType, string> volumeNameDictionary = new()
        {
            { VolumeType.Master, "MasterVolume" },
            { VolumeType.BGM, "BGMVolume" },
            { VolumeType.SFX, "SFXVolume" }
        };

        public static string GetExposedVolumeName(VolumeType volumeType)
        {
            return volumeNameDictionary[volumeType];
        }
    }




    public enum AIState
    {
        Idle, // 가만히 있기
        Flee, // 도망치기
        Attack, // 공격하기
        Return // 귀환하기
    }


    public static class AnimatorHash
    {
        // NPC
        public static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
        public static readonly int AttackHash = Animator.StringToHash("Attack");
        public static readonly int HitHash = Animator.StringToHash("Hit");
        public static readonly int DieHash = Animator.StringToHash("Die");
    }

    public enum BuildMode
    {
        None = 10000, Floor = 10001, Wall = 10002, Stair = 10003
    }

    public enum Direction
    {
        North, South, East, West
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
            if (paths.ContainsKey(prefabName))
            {
                return paths[prefabName];
            }
            return "";
        }
    }
}
