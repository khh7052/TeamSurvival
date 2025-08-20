using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    public int dmg;
    public float dmgRate;

    private List<IDamageable> things = new();

    private void Start()
    {
        InvokeRepeating("DealDamage", 0, dmgRate);
    }

    void DealDamage()
    {
        for(int i = 0; i < things.Count; i++)
        {
            things[i].TakePhysicalDamage(dmg);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IDamageable damageable))
        {
            things.Add(damageable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
        {
            things.Remove(damageable);
        }
    }
}
