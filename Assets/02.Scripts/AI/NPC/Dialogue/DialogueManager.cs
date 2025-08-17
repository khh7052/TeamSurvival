using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DialogueManager : Singleton<DialogueManager>
{
    private Action EndDialogueAction;

    [SerializeField] private GameObject dialoguePanel; // 대화 UI 패널
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
            Debug.LogWarning("이미 대화 중입니다.");
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
            StopCoroutine(typingCoroutine); // 이전 타이핑 코루틴 중지
            typingCoroutine = null;
        }

        DialogueLine line = currentDialogue.lines[index];
        // UI에 출력
        spekaerText.text = line.speaker;

        if(line.useTypingEffect)
            typingCoroutine = StartCoroutine(TypingText(line.text, line.typingSpeed));
        else
            dialogueText.text = line.text; // 타이핑 효과 없이 바로 출력
    }

    private void EndDialogue()
    {
        currentDialogue = null;
        dialoguePanel.SetActive(false);
        Debug.Log("대화 종료");

        EndDialogueAction?.Invoke();
    }

    IEnumerator TypingText(string text, float typingSpeed)
    {
        dialogueText.text = ""; // 텍스트 초기화
        foreach (char c in text)
        {
            dialogueText.text += c; // 한 글자씩 추가

            // 공백이나 줄바꿈 문자가 아닌 경우에만 대기
            if (c != ' ' && c != '\n')
                yield return new WaitForSeconds(typingSpeed); // 타이핑 속도만큼 대기
        }
    }
}
