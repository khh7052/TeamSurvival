using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipSystem : MonoBehaviour
{
    //[SerializeField] private EquipItem currentEquip;
    //[SerializeField] private Transform equipParent;

    //private GameObject spawned;

    //public void UseCurrentItem(GameObject target)
    //{
    //    if (currentEquip == null)
    //    {
    //        return;
    //    }

    //    switch (currentEquip.itemType)
    //    {
    //        case ItemType.Weapon :
    //            Monster monster = target.GetComponent<Monster>();
    //            if (monster != null)
    //            {
    //                monster.TakeDamage(currentEquip.damage);
    //            }
    //            else
    //            {
    //                Debug.Log("공격 불가");
    //            }
    //            break;

    //        case ItemType.Tool :
    //            ResourceNode resource = target.GetComponent<ResourceNode>();
    //            if (resource != null)
    //            {
    //                resource.Gather(currentEquip.gatheringPower);
    //            }
    //            else
    //            {
    //                Debug.Log("채취 불가");
    //            }
    //            break;
    //    }
    //}

    //public void EquipItem()
    //{
    //    UnEquipItem();

    //    if (currentEquip == null || currentEquip.equipPrefab == null) return;

    //    Transform parent = equipParent != null ? equipParent : transform;
    //    spawned = Instantiate(currentEquip.equipPrefab, parent);
    //    spawned.transform.localPosition = Vector3.zero;
    //    spawned.transform.localRotation = Quaternion.identity;
    //}

    //public void UnEquipItem()
    //{
    //    if (spawned != null)
    //    {
    //        Destroy(spawned);
    //        spawned = null;
    //    }
    //}
}
