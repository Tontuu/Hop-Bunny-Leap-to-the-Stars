using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChargeHandler : MonoBehaviour
{
    private PlayerController player;

    // Reference to the input actions
    private PlayerInputAssets inputActions;

    // Charge value
    public float chargeValue;
    private bool forceRelease;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        inputActions = new PlayerInputAssets();

        // Register event handlers
        inputActions.Gameplay.Jump.started += OnChargeStarted;
        inputActions.Gameplay.Jump.canceled += OnChargeCanceled;
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

    private void Update()
    {
        if (player.isCharging)
        {
            // Player cannot move while charging
            player.rb.velocity = new Vector2(0, 0);

            // Increase charge value
            chargeValue += Constants.JUMP_CHARGE_MAGNITUDE; // Adjust the speed as needed

            // Check if charge reaches 100
            if (chargeValue >= Constants.MAX_JUMP_MAGNITUDE + Constants.DELAY_JUMP && !forceRelease)
            {
                forceRelease = true;
                OnChargeCanceled(default);
            }
        }

    }

    private void OnChargeStarted(InputAction.CallbackContext context)
    {
        // Start charging
        SoundManager.Instance.PlaySound2D("Charging");
        player.isCharging = player.isGrounded ? true : false;
        chargeValue = 0;
        forceRelease = false;
    }

    private void OnChargeCanceled(InputAction.CallbackContext context)
    {
        // Stop charging and send the message
        if (player.isCharging || forceRelease)
        {
            chargeValue = Mathf.Clamp(chargeValue, Constants.MIN_JUMP_MAGNITUDE, Constants.MAX_JUMP_MAGNITUDE);

            player.isCharging = false;
            player.isJumping = true;

            JumpCounterController.Instance.IncrementCounter();
            SoundManager.Instance.PlaySound2D("Jump");

            player.CreateDust(chargeValue);
            player.rb.velocity = new Vector2(player.dir * Constants.HORIZONTAL_JUMP_SPEED, chargeValue);
            player.SetFacingDirection(player.dir);

            // Reset the charge value
            chargeValue = 0;
            forceRelease = false;
        }
    }
}
