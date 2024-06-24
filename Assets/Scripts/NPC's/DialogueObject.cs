using System.Collections;
using UnityEngine;

[System.Serializable]
public class DialogueObject
{
    public string audioclip;
    public string npcName;
    [TextArea(3, 10)]
    public string[] sentences;
    public Vector2 boxOffset;
}