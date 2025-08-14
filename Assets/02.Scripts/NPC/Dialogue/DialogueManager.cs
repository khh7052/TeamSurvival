using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DialogueManager : Singleton<DialogueManager>
{
    private Action EndDialogueAction;

    [SerializeField] private GameObject dialoguePanel; // ��ȭ UI �г�
    [SerializeField] private TMP_Text spekaerText;
    [SerializeField] private TMP_Text dialogueText;
    private DialogueData currentDialogue;
    private int index;

    public bool IsDialogueActive => currentDialogue != null;

    protected override void Initialize()
    {
        base.Initialize();
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(DialogueData dialogue, Action callback)
    {
        if (IsDialogueActive)
        {
            Debug.LogWarning("�̹� ��ȭ ���Դϴ�.");
            return;
        }

        dialoguePanel.SetActive(true);
        currentDialogue = dialogue;
        index = 0;
        ShowLine();

        EndDialogueAction = callback;
    }

    public void NextLine()
    {
        if (++index < currentDialogue.lines.Length)
            ShowLine();
        else
            EndDialogue();
    }

    private void ShowLine()
    {
        DialogueLine line = currentDialogue.lines[index];
        // UI�� ���
        spekaerText.text = line.speaker;
        dialogueText.text = line.text;
    }

    private void EndDialogue()
    {
        currentDialogue = null;
        dialoguePanel.SetActive(false);
        Debug.Log("��ȭ ����");

        EndDialogueAction?.Invoke();
    }
}
