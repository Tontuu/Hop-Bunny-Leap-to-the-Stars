using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPCInteractionHandler : MonoBehaviour
{
    public DialogueManager dialogueManager;

    // Reference to the input actions
    private PlayerInputAssets inputActions;

    private void Awake()
    {
        dialogueManager = GetComponent<DialogueManager>();
        inputActions = new PlayerInputAssets();

        // Register event handler
        inputActions.Gameplay.Interact.started += OnInteract;

    }

    private void OnEnable()
    {
        // Enable the input actions
        inputActions.Enable();
    }

    private void OnDisable()
    {
        // Disable the input actions
        inputActions.Disable();
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
        {
            if (dialogueManager.isDialogueActive)
            {
                dialogueManager.InteractWithNpc();
            }
        }
    }
}