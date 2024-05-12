using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public Image characterIcon;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;
    public GameObject tutorialOverlay;
    private bool finishedTutorial = false;

    private Queue<DialogueLine> lines;

    public bool isDialogueActive = false;
    [SerializeField]
    public float typingSpeed = 0.030f;
    public Animator animator;
    public Animator spriteAnimator;
    private float elapsedTime;
    bool isKeyPressed = false;

    private void Awake()
    {
        // We'll be using only one instance of this class
        if (Instance == null)
        {
            Instance = this;
        }

        lines = new Queue<DialogueLine>();
    }

    private void Update()
    {
        if (isDialogueActive)
        {
            if (Input.GetKeyUp(KeyCode.E))
            {
                isKeyPressed = true;
                if (!finishedTutorial)
                {
                    finishedTutorial = true;
                    tutorialOverlay.GetComponent<Animator>().Play("press-e-hide");
                }

                elapsedTime = 0f;
                DisplayNextDialogueLine();
                isKeyPressed = false;
            }
            if (finishedTutorial)
            {
                elapsedTime += Time.deltaTime;
            }

            if (elapsedTime >= 15.0f && !isKeyPressed)
            {
                finishedTutorial = false;
                elapsedTime = 0f;
                tutorialOverlay.GetComponent<Animator>().Play("press-e-show");
            }
        }
        Debug.Log(isDialogueActive);
    }

    public bool CanDisableTrigger()
    {
        return isDialogueActive;
    }

    public void StartDialogue(Dialogue dialogue)
    {
        tutorialOverlay.SetActive(true);
        isDialogueActive = true;
        animator.SetTrigger("start");
        lines.Clear();

        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }
        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = lines.Dequeue();

        int charactersCount = currentLine.line.Length;
        float mappedTypingSpeed = Mathf.Lerp(0.03f, 0.1f, Mathf.InverseLerp(60, 1, charactersCount));
        typingSpeed = mappedTypingSpeed;

        spriteAnimator.Play(currentLine.character.name.ToLower() + "-portrait");
        characterIcon.sprite = currentLine.character.icon;
        characterName.text = currentLine.character.name;

        StopAllCoroutines();

        StartCoroutine(TypeSentence(currentLine));
    }

    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        dialogueArea.text = "";
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            dialogueArea.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        animator.Play("dialogue-hide-anim");
    }
}