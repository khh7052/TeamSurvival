using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speaker; // ȭ��
    [TextArea]
    public string text; // ��� ����
    public bool useTypingEffect = true; // Ÿ���� ȿ�� ��� ����
    public float typingSpeed = 0.1f; // Ÿ���� �ӵ� (�� ����)
}
