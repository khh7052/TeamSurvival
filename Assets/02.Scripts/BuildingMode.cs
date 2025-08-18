using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMode : MonoBehaviour
{
    public bool isBuild = false;
    public float rayDistance;
    public LayerMask buildMask;
    public BuildMode buildMode;

    public void Update()
    {
        if (isBuild)
        {
            Ray(out RaycastHit hit, rayDistance);
            if (hit.collider != null)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    CreateBuildObj(hit, buildMode);
                }
            }
        }
    }

    private bool Ray(out RaycastHit hit, float distance)
    {
        Vector3 origin, dir;
        if (Camera.main != null)
        {
            origin = Camera.main.transform.position;
            dir = Camera.main.transform.forward;
        }
        else
        {
            origin = transform.position + Vector3.up * 0.8f;
            dir = transform.forward;
        }
        return Physics.Raycast(origin, dir, out hit, distance, buildMask);
    }

    public async void CreateBuildObj(RaycastHit hit, BuildMode mode)
    {
        var (pos, dir, rot) = BuildingManager.Instance.GetBuildPos(hit.point);
        BuildKey key = new(mode, pos, dir);
        // 해당 위치에 이미 건설된 상태일 경우
        if (BuildingManager.Instance.IsOccupied(key))
        {
            Debug.Log($"이미 {mode} 가 설치된 자리입니다!");
            return;
        }

        GameObject go = await Factory.Instance.CreateByIDAsync<BaseScriptableObject>((int)mode, (go) =>
        {
            var obj = go.AddComponent<BuildObj>();
            obj.key = key;
            obj.Initialize();
            obj.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
        });

        go.transform.SetPositionAndRotation(pos, rot);

        BuildingManager.Instance.RegisterBuild(key);
    }

}
