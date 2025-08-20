using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotObject : MonoBehaviour //근처에 가면 체온회복 class
{
    public float detectionRadius = 5f;
    public LayerMask layerMask;

    public float healRate = 1f;
    private float timer = 0f;

    void Update()
    {
        RayCastPlayer();
    }

    void RecoverTemperture(EntityModel model)
    {
        timer += Time.deltaTime;

        if(timer > healRate)
        {
            model.temperture.Add(0.01f);
            Debug.Log("따뜻하다. 체온이 오른다");
            timer = 0f;
        }      
    }

    void RayCastPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, layerMask);

        foreach (var hit in hits)
        {
            Debug.Log($"{hit.name}이 범위 안에 있습니다.");

            var obj = hit.GetComponent<EntityModel>();
            if(obj != null )
            {
                RecoverTemperture(obj);
            }   
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
