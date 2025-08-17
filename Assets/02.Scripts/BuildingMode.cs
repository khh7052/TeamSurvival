using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMode : MonoBehaviour
{
    public bool isBuild = false;
    public float rayDistance;
    public LayerMask buildMask;

    public void Update()
    {
        if (isBuild)
        {
            Ray(out RaycastHit hit, rayDistance);
            if (hit.collider != null)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    CreateBuildObj(hit);
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

    public async void CreateBuildObj(RaycastHit hit)
    {
        GameObject go = await Factory.Instance.CreateByIDAsync<BaseScriptableObject>(10002);
        go.transform.SetPositionAndRotation(hit.point, Quaternion.identity);
    }

}
