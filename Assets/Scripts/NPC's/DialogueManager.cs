using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public GameObject canvas;
    public GameObject DialogueBoxPrefab;
    public GameObject tutorialOverlay;
    public List<GameObject> openDialogueObjects;
    public Queue<string> sentences;
    public Queue<string> npcs;

    static bool isDialogueActive;
    private bool finishedTutorial = false;
    private bool isKeyPressed = false;
    private float elapsedTime;
    public float typingSpeed = 0.030f;


    void Update()
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
                DisplayNextDialogue();
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
    }

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        sentences = new Queue<string>();
        npcs = new Queue<string>();
        openDialogueObjects = new List<GameObject>();
        isDialogueActive = false;
    }

    public void StartDialogue(DialogueObject dialogue, Vector2 offset)
    {
        foreach (GameObject dialogueObject in openDialogueObjects)
        {
            if (dialogueObject != null)
            {
                dialogueObject.transform.GetChild(0).GetComponent<Animator>().Play("dialogue-hide-anim");
            }
            Invoke("DestroyDialogueObjects", 3);
        }

        tutorialOverlay.GetComponent<Animator>().Play("press-e-show");
        isDialogueActive = true;
        sentences.Clear();
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        npcs.Enqueue(dialogue.npcName);

        GameObject dialogueContainer = new GameObject("DialogueContainer-" + dialogue.npcName);
        dialogueContainer.transform.position = dialogue.boxOffset;
        dialogueContainer = Instantiate(dialogueContainer, offset + dialogue.boxOffset, Quaternion.identity, canvas.transform);
        GameObject dialogueBox = Instantiate(DialogueBoxPrefab, dialogueContainer.transform.position, Quaternion.identity, dialogueContainer.transform);
        dialogueBox.GetComponent<Animator>().Play("dialogue-show-anim");
        openDialogueObjects.Add(dialogueContainer);

        DisplayNextDialogue();
    }
    public bool CanDisableTrigger()
    {
        return isDialogueActive;
    }

    public void DisplayNextDialogue()
    {
        string npcName = "Default";
        TextMeshProUGUI characterName;
        TextMeshProUGUI dialogueArea;
        Animator spriteAnimator;

        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        GameObject dialogue = openDialogueObjects.Last();
        characterName = dialogue.transform.GetChild(0).transform.Find("Header").GetChild(0).GetComponent<TextMeshProUGUI>();
        dialogueArea = dialogue.transform.GetChild(0).transform.Find("Body").GetChild(0).GetComponent<TextMeshProUGUI>();
        spriteAnimator = dialogue.transform.GetChild(0).transform.Find("Icon").GetChild(0).GetComponent<Animator>();

        if (npcs.Count != 0)
        {
            npcName = npcs.Dequeue();
        }
        string sentence = sentences.Dequeue();
        int charactersCount = sentence.Length;
        float mappedTypingSpeed = Mathf.Lerp(0.03f, 0.1f, Mathf.InverseLerp(80, 1, charactersCount));
        typingSpeed = mappedTypingSpeed;
        spriteAnimator.Play(npcName.ToLower() + "-portrait");
        characterName.text = npcName;

        StopAllCoroutines();

        StartCoroutine(TypeSentence(sentence, dialogueArea));
    }

    IEnumerator TypeSentence(string sentence, TextMeshProUGUI dialogueArea)
    {
        dialogueArea.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueArea.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private void EndDialogue()
    {
        foreach (GameObject dialogueObject in openDialogueObjects)
        {
            if (dialogueObject != null)
            {
                dialogueObject.transform.GetChild(0).GetComponent<Animator>().Play("dialogue-hide-anim");
                StartCoroutine(SelfDestruct(dialogueObject));
            }
        }
        isDialogueActive = false;
    }

    IEnumerator SelfDestruct(GameObject dialogueObject)
    {
        yield return new WaitForSeconds(3f);
        Destroy(dialogueObject);
    }

    private void DestroyDialogueObjects()
    {
        for (int index = 0; index < openDialogueObjects.Count - 1; index++)
        {
            Destroy(openDialogueObjects[index]);
        }
    }
}