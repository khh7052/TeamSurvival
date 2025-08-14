using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speaker; // 화자
    [TextArea]
    public string text; // 대사 내용
}
