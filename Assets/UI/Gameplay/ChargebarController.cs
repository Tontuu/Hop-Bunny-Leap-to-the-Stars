using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class ChargebarController : MonoBehaviour
{
    private Animator animator;
    private int numberOfBars = 5;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }

    private int GetChargeBarIndex(float chargeValue, float max_value)
    {
        int indexBar = 0;
        if (chargeValue < max_value / numberOfBars && chargeValue > 0) { indexBar++; }
        if (chargeValue > max_value / numberOfBars) { indexBar++; }
        if (chargeValue > max_value / numberOfBars * 2) { indexBar++; }
        if (chargeValue > max_value / numberOfBars * 3) { indexBar++; }
        if (chargeValue > max_value / numberOfBars * 4) { indexBar++; }
        if (chargeValue >= max_value / numberOfBars * 5) { indexBar++; }

        return indexBar;
    }

    private void UpdateChargebar()
    {
        int indexBar = GetChargeBarIndex(PlayerController.chargeValue, Constants.MAX_JUMP_MAGNITUDE);
        if (indexBar == 1)
        {
            animator.Play("chargebar-anim", 0, 1);
        }
        if (indexBar == 2)
        {
            animator.Play("chargebar-anim", 0, 2);
        }
    }

    void Update()
    {
        UpdateChargebar();
    }
}
