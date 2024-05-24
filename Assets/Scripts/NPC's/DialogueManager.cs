using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public GameObject canvas;
    public GameObject DialogueBoxPrefab;
    public List<GameObject> openDialogueObjects;
    public Queue<string> sentences;
    public GameObject tutorialOverlay;
    static bool isDialogueActive;

    void Update()
    {
        if (isDialogueActive)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                tutorialOverlay.GetComponent<Animator>().Play("press-e-hide");
                DisplayNextDialogue();
            }
        }
    }

    void Start()
    {
        sentences = new Queue<string>();
        openDialogueObjects = new List<GameObject>();
        isDialogueActive = false;
    }

    public void StartDialogue(DialogueObject dialogue)
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
        Debug.Log("DEBUG: Starting conversation with " + dialogue.npcName);
        sentences.Clear();
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        GameObject dialogueContainer = new GameObject("DialogueContainer-" + dialogue.npcName);
        dialogueContainer.transform.position = dialogue.boxOffset;
        dialogueContainer = Instantiate(dialogueContainer, canvas.transform);
        GameObject dialogueBox = Instantiate(DialogueBoxPrefab, dialogue.boxOffset, Quaternion.identity, dialogueContainer.transform);
        dialogueBox.GetComponent<Animator>().Play("dialogue-show-anim");
        openDialogueObjects.Add(dialogueContainer);

        DisplayNextDialogue();
    }

    public void DisplayNextDialogue()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        Debug.Log("DEBUG: " + sentence);
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