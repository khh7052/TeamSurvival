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
    private Coroutine typingCoroutine;

    public bool IsDialogueActive => currentDialogue != null;

    protected override void Initialize()
    {
        base.Initialize();
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(DialogueData dialogue, Action callback = null)
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
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine); // ���� Ÿ���� �ڷ�ƾ ����
            typingCoroutine = null;
        }

        DialogueLine line = currentDialogue.lines[index];
        // UI�� ���
        spekaerText.text = line.speaker;

        if(line.useTypingEffect)
            typingCoroutine = StartCoroutine(TypingText(line.text, line.typingSpeed));
        else
            dialogueText.text = line.text; // Ÿ���� ȿ�� ���� �ٷ� ���
    }

    private void EndDialogue()
    {
        currentDialogue = null;
        dialoguePanel.SetActive(false);
        Debug.Log("��ȭ ����");

        EndDialogueAction?.Invoke();
    }

    IEnumerator TypingText(string text, float typingSpeed)
    {
        dialogueText.text = ""; // �ؽ�Ʈ �ʱ�ȭ
        foreach (char c in text)
        {
            dialogueText.text += c; // �� ���ھ� �߰�

            // �����̳� �ٹٲ� ���ڰ� �ƴ� ��쿡�� ���
            if (c != ' ' && c != '\n')
                yield return new WaitForSeconds(typingSpeed); // Ÿ���� �ӵ���ŭ ���
        }
    }
}
