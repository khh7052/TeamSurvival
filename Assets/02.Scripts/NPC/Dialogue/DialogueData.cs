using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Data/DialogueData")]
public class DialogueData : ScriptableObject
{
    public DialogueLine[] lines; // 대사 목록

}
