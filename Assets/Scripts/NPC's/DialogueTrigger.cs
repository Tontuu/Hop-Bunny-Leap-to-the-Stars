using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class DialogueCharacter
{
    public string name;
    public Sprite icon;
}

[System.Serializable]
public class DialogueLine
{
    public DialogueCharacter character;
    [TextArea(3, 10)]
    public string line;
}

[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField]
    public Vector2 canvasPos;
    public GameObject DialogueCanvasPrefab;
    public GameObject DialogueCanvas;
    public Dialogue dialogue;


    public void TriggerDialogue()
    {
        if (DialogueCanvas == null)
        {
            DialogueCanvas = Instantiate(DialogueCanvasPrefab, canvasPos, Quaternion.identity);
            DialogueManager.Instance.StartDialogue(dialogue);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            TriggerDialogue();
        }
    }
}
