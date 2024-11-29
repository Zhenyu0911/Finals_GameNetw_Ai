using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text turnIndicatorText;
    [SerializeField] private TMP_Text actionFeedText;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button abilityButton;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private Slider healthSlider;

    private int maxHealth = 100;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    public void UpdateTurnIndicator(string message)
    {
        turnIndicatorText.text = message;
    }

    public void UpdateActionFeed(string message)
    {
        actionFeedText.text = message;
    }

    public void UpdateActionButtons(bool isInteractable)
    {
        attackButton.interactable = isInteractable;
        abilityButton.interactable = isInteractable;
        endTurnButton.interactable = isInteractable;
    }

    public void UpdateHealthSlider(int health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        healthSlider.value = currentHealth;
    }

    public void LogAction(string actionMessage)
    {
        if (actionFeedText != null)
        {
            actionFeedText.text = actionMessage; // Update the action feed text
        }
    }
}
