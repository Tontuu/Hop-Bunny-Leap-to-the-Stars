using UnityEngine.UI;
using UnityEngine;

public class ChargebarController : MonoBehaviour
{
    [SerializeField] public Sprite[] sprite_bars;
    private Animator animator;
    private int numberOfBars = 6;
    private bool startedToCharge = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }

    void UpdateChargebar()
    {
        int bar_index = GetChargeBarIndex(PlayerController.chargeValue, Constants.MAX_JUMP_MAGNITUDE);
        if (bar_index <= numberOfBars)
        {
            gameObject.GetComponent<Image>().sprite = sprite_bars[bar_index];
        }

        if (bar_index == numberOfBars)
        {
            animator.enabled = true;
            animator.Play("chargebar-charged");
        }

        if (PlayerController.isCharging == false)
        {
            animator.enabled = false;
            gameObject.GetComponent<Image>().sprite = sprite_bars[0];
        }
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
        if (chargeValue >= max_value / numberOfBars * 6) { indexBar++; }

        return indexBar;
    }

    void Update()
    {
        UpdateChargebar();
    }
}
