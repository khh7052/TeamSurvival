using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
    public async void Initialize()
    {
        if (UIManager.Instance.IsEnableUI<InGameUI>())
        {
            Debug.Log("이미 있네?");
            UIManager.Instance.CloseUI<InGameUI>();
        }
        var obj = await UIManager.Instance.ShowUI<InGameUI>();
        InGameUI ui = obj as InGameUI;
        GameManager.player.interaction.OnDetectRay += obj.SetPromptText;
        GameManager.player.interaction.OnEndDetectRay += obj.EndPromptText;
    }
}