using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData data;

    public string GetPrompt()
    {
        if (data == null) return "Item";

        string nameTitle = string.IsNullOrEmpty(data.name) ? "Item" : data.name;
        string desc = string.IsNullOrWhiteSpace(data.description) ? "" : data.description;

        return string.IsNullOrEmpty(desc) ? nameTitle : $"{nameTitle}\n{desc}";
    }



    public void OnInteract()
    {
        //인벤토리에 추가
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.Add(data);
        }
        else
        {
            Debug.LogWarning("Player에 PlayerInventory를 붙였는지 확인");
            return; // 인벤토리가 없으면 파괴하지 않음(디버그 편의)
        }

        //제거
        gameObject.SetActive(false);
    }
}
