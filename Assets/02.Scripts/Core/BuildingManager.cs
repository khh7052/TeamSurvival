using Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BuildingManager : Singleton<BuildingManager>, IInitializableAsync
{
    private ScriptableObjectDataBase<BaseScriptableObject> database; 
    public bool IsInitialized { get; private set; }

    private object lockObj = new();
    [SerializeField]
    private float gridSize = 1f;

    // 건축 데이터 해쉬셋
    private HashSet<BuildKey> occupied = new();

    public async void InitializeAsync()
    {
        database = new ScriptableObjectDataBase<BaseScriptableObject>();
        await database.Initialize("BuildingItem");
        IsInitialized = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
        InitializeAsync();
        
    }

    public (Vector3, Direction, Quaternion) GetBuildPos(Vector3 pos)
    {
        lock (lockObj)
        {
            int gridX, gridY, gridZ;
            gridX = Mathf.RoundToInt(pos.x / gridSize);
            gridY = Mathf.RoundToInt(pos.y / gridSize);
            gridZ = Mathf.RoundToInt(pos.z / gridSize);

            Vector3 retPos = new Vector3(gridX * gridSize, gridY * gridSize, gridZ * gridSize);

            // --- 회전 계산 ---
            Quaternion rotation = Quaternion.identity;

            // XZ 평면에서 4방향 회전
            Vector2 dir = new Vector2(pos.x - retPos.x, pos.z - retPos.z).normalized;
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                // 좌/우
                rotation = dir.x > 0 ? Quaternion.Euler(0, 90, 0) : Quaternion.Euler(0, -90, 0);
            }
            else
            {
                // 앞/뒤
                rotation = dir.y > 0 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
            }

            // Y축 위/아래 건설 (예: 바닥/천장)
            float offsetY = pos.y - retPos.y;
            if (offsetY > gridSize * 0.5f)
            {
                // 위쪽에 건설 (천장 붙이기)
                rotation *= Quaternion.Euler(90, 0, 0);
            }
            else if (offsetY < -gridSize * 0.5f)
            {
                // 아래쪽에 건설 (바닥 붙이기)
                rotation *= Quaternion.Euler(-90, 0, 0);
            }
            Direction retDir = SnapToDirection(rotation);

            return (retPos, retDir, rotation);
        }
    }

    public Direction SnapToDirection(Quaternion rot)
    {
        Vector3 forward = rot * Vector3.forward;
        Vector3[] dirs = { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };
        Direction[] enums = { Direction.North, Direction.South, Direction.East, Direction.West };

        float maxDot = -1f;
        int best = 0;
        for (int i = 0; i < dirs.Length; i++)
        {
            float dot = Vector3.Dot(forward.normalized, dirs[i]);
            if (dot > maxDot)
            {
                maxDot = dot;
                best = i;
            }
        }
        return enums[best];
    }

    public bool IsOccupied(BuildKey key)
    {
        bool ret = occupied.Contains(key);
        return ret;
    }

    public void RegisterBuild(BuildKey key) => occupied.Add(key);

    public void UnregisterBuild(BuildKey key) => occupied.Remove(key);
}

[Serializable]
public struct BuildKey : IEquatable<BuildKey>
{
    public BuildMode Mode;
    public Vector3 Position;  // Grid 좌표
    public Direction? Dir;       // Wall만 사용

    public BuildKey(BuildMode mode, Vector3 pos, Direction? dir = null)
    {
        Mode = mode;
        Position = pos;
        Dir = dir;
        Normalize(ref this);
    }

    /// <summary>
    /// West, South 방향 벽을 항상 East, North로 하여 인접한 그리드의 다른 방향 오브젝트를 동일화
    /// </summary>
    /// <param name="key"></param>
    private void Normalize(ref BuildKey key)
    {
        if (key.Mode != BuildMode.Wall || key.Dir == null) return;

        switch (key.Dir.Value)
        {
            case Direction.West: // (-X) → 왼쪽 벽은 좌표를 -1 이동 후 East로 통일
                key.Position += Vector3Int.left;
                key.Dir = Direction.East;
                break;

            case Direction.South: // (-Z) → 뒤쪽 벽은 좌표 -1 이동 후 North로 통일
                key.Position += Vector3Int.back;
                key.Dir = Direction.North;
                break;
        }
    }

    public bool Equals(BuildKey other)
    {
        return Mode == other.Mode &&
               Position == other.Position &&
               (Mode != BuildMode.Wall || Dir == other.Dir);
    }

    public override int GetHashCode()
    {
        int hash = Mode.GetHashCode() ^ Position.GetHashCode();
        if (Mode == BuildMode.Wall && Dir != null)
            hash ^= Dir.GetHashCode();
        return hash;
    }
}