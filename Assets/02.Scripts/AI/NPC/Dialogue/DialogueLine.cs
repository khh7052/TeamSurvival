using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speaker; // 화자
    [TextArea]
    public string text; // 대사 내용
    public bool useTypingEffect = true; // 타이핑 효과 사용 여부
    public float typingSpeed = 0.1f; // 타이핑 속도 (초 단위)
}
