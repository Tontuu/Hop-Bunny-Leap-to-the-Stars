using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPCInteractionHandler : MonoBehaviour
{
    public Animator tutorialImage;
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

    public void Update()
    {
        if (Gamepad.all.Count > 0)
        {
            tutorialImage.SetBool("Gamepad", true);
        }
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

    public void OnJoin(InputAction.CallbackContext ctx)
    {
        if (ctx.control.device is Keyboard)
        {
            tutorialImage.SetBool("Gamepad", false);
        }
        else if (ctx.control.device is Gamepad)
        {
            tutorialImage.SetBool("Gamepad", true);
        }
    }
}