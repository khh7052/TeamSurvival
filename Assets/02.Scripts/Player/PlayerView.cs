using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
    public async void Initialize()
    {
        var obj = await UIManager.Instance.ShowUI<InGameUI>();
        InGameUI ui = obj as InGameUI;
        GameManager.player.interaction.OnDetectRay += obj.SetPromptText;
        GameManager.player.interaction.OnEndDetectRay += obj.EndPromptText;
    }
}