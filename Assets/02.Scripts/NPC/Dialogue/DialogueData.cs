using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Data/DialogueData")]
public class DialogueData : BaseScriptableObject
{
    public DialogueLine[] lines; // 대사 목록

}
