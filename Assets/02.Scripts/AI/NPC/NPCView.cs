using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class NPCView : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;

    public void SetName(string name)
    {
        if (nameText == null) return;
        nameText.text = name;
    }

}
