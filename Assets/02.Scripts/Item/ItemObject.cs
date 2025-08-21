using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData data;

    public string GetPrompt()
    {
        string nameTitle = data ? data.name : "Item";
        string desc = (data && !string.IsNullOrWhiteSpace(data.description)) ? data.description : "";
        return string.IsNullOrEmpty(desc) ? nameTitle : $"{nameTitle}\n{desc}";
    }



    public void OnInteract()
    {
        var player = FindFirstObjectByType<Player>(); // ���� 1�� �ִٰ� ����
        if (player == null || data == null)
        {
            Debug.LogWarning("[ItemObject] Player �Ǵ� ItemData ����");
            return;
        }

        player.itemData = data;
        player.addItem?.Invoke();
        gameObject.SetActive(false);
    }
}
