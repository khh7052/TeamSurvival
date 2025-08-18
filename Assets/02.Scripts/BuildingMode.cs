using Constants;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class BuildingMode : MonoBehaviour
{
    public bool isBuild = false;
    public float rayDistance;
    public LayerMask buildMask;
    public BuildMode buildMode;

    private BuildKey buildKey;
    private GameObject preViewObj;

    private GameObject[] preViewObjs = new GameObject[2];

    private async void Start()
    {
        while (!Factory.Instance.IsInitialized)
        {
            await Task.Yield();
        }
        Debug.Log("준비 완료");

        preViewObjs[0] = await Factory.Instance.CreateByIDAsync<BaseScriptableObject>(10011, (go) =>
        {
            go.SetActive(false);
            go.GetComponentInChildren<MeshRenderer>().material.color = new Color(0, 0, 1, 0.6f);
        });
        preViewObjs[1] = await Factory.Instance.CreateByIDAsync<BaseScriptableObject>(10012, (go) =>
        {
            go.SetActive(false);
            go.GetComponentInChildren<MeshRenderer>().material.color = new Color(0, 0, 1, 0.6f);
        });
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            isBuild = !isBuild;
        }
        if (isBuild)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                DestroyPrevObj();
                buildMode = BuildMode.Floor;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                DestroyPrevObj();
                buildMode = BuildMode.Wall;
            }

            Ray(out RaycastHit hit, rayDistance);
            if (hit.collider != null)
            {
                var (pos, dir, rot) = BuildingManager.Instance.GetBuildPos(hit.point);
                BuildKey newKey = new(buildMode, pos, dir);

                // 해당 위치에 이미 건설된 상태일 경우
                if (BuildingManager.Instance.IsOccupied(newKey))
                {
                    DestroyPrevObj();
                    Debug.Log($"이미 {buildMode} 가 설치된 자리입니다!");
                    return;
                }

                if(preViewObj == null || !buildKey.Equals(newKey))
                {
                    DestroyPrevObj();
                    buildKey = newKey;
                    CreatePreviewObj(hit, rot, buildMode);
                }


                if (Input.GetKeyDown(KeyCode.F))
                {
                    CreateBuildObj(hit, rot, buildMode);
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

    private void DestroyPrevObj()
    {
        if (preViewObj != null)
        {
            preViewObj.SetActive(false);
            preViewObj = null;
        }
    }

    public void CreatePreviewObj(RaycastHit hit, Quaternion rot, BuildMode mode)
    {
        GameObject go = preViewObjs[((int)mode - 10001)];

        go.transform.SetPositionAndRotation(buildKey.Position, buildKey.rot);
        go.SetActive(true);
        preViewObj = go;
    }

    public async void CreateBuildObj(RaycastHit hit, Quaternion rot, BuildMode mode)
    {


        GameObject go = await Factory.Instance.CreateByIDAsync<BaseScriptableObject>((int)mode, (go) =>
        {
            var obj = go.AddComponent<BuildObj>();
            obj.key = buildKey;
            obj.Initialize();
            obj.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
        });

        go.transform.SetPositionAndRotation(buildKey.Position, buildKey.rot);

        BuildingManager.Instance.RegisterBuild(buildKey);
    }

}
