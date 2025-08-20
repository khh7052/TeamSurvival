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

    protected override async void Initialize()
    {
        base.Initialize();
        var dialogue = await UIManager.Instance.ShowUI<DialogueUI>();
        speakerText = dialogue.speakerText;
        dialogueText = dialogue.scriptText;
        UIManager.Instance.CloseUI<DialogueUI>();
    }

    private void Update()
    {
        // ��ȭâ�� Ȱ��ȭ�Ǿ� ���� ������ �ƹ��͵� ���� ����
        if (!IsDialogueActive)
            return;

        // EŰ�� ���� ��ȭ
        if (Input.GetKeyDown(KeyCode.E))
        {
            // �ؽ�Ʈ ��� ��
            if (typingCoroutine != null)
            {
                // Ÿ���� �ڷ�ƾ�� �����ϰ� ��ü �ؽ�Ʈ�� �ٷ� ǥ��
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
                dialogueText.text = currentDialogue.lines[index].text;
            }
            // Ÿ������ ���� ���¶��
            else
            {
                // ���� ��縦 ������
                NextLine();
            }
        }
    }

    public async void StartDialogue(DialogueData dialogue, Action callback = null)
    {
        if (IsDialogueActive)
        {
            Debug.LogWarning("�̹� ��ȭ ���Դϴ�.");
            return;
        }

        await UIManager.Instance.ShowUI<DialogueUI>();
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
