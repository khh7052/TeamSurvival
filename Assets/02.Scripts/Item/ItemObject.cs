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
        //�κ��丮�� �߰�
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.Add(data);
        }
        else
        {
            Debug.LogWarning("Player�� PlayerInventory�� �ٿ����� Ȯ��");
            return; // �κ��丮�� ������ �ı����� ����(����� ����)
        }

        //����
        gameObject.SetActive(false);
    }
}
