using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class LookUpInteraction : MonoBehaviour
{
    // Reference to the input actions
    private PlayerInputAssets inputActions;
    public bool alreadyCreatedCam = false;

    PlayerController player;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        inputActions = new PlayerInputAssets();

        // Register event handler
        inputActions.Gameplay.Lookup.started += OnLookup;
        inputActions.Gameplay.Lookup.performed += OnLookup;
        inputActions.Gameplay.Lookup.canceled += OnLookup;
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

    public void OnLookup(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)        
        {
            player.isLookingUp = true;
        }

        if (ctx.canceled)
        {
            player.isLookingUp = false;
            player.alreadyCreatedCam = false;
            Invoke("ResetCameraBlend", 0.5f);
        }
    }
    void ResetCameraBlend()
    {
        player.mainCam.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = 0.1f;
    }
}