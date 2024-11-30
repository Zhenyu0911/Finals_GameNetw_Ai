using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamHP : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] public float MaxHP = 100.0f;
    [SerializeField] public float CurrentHP;

    [Header("UI Components")]
    [SerializeField] private Slider healthSlider; // Reference to the health slider UI

    [Header("References")]
    [SerializeField] private Enemy enemy; // Reference to an Enemy script (or use other means to calculate damage)

    void Start()
    {
        ResetHP();

        // Ensure the slider is configured correctly
        if (healthSlider != null)
        {
            healthSlider.maxValue = MaxHP;
            healthSlider.value = CurrentHP;
        }
    }

    void Update()
    {
        if (enemy != null) // Ensure enemy reference is set
        {
            TakeDamageFromEnemy();
        }

        UpdateHealthSlider();
        NoHealthPlayer();
    }

    private void TakeDamageFromEnemy()
    {
        if (enemy != null)
        {
            CurrentHP -= enemy.Damage * Time.deltaTime; // Apply enemy damage over time
            CurrentHP = Mathf.Clamp(CurrentHP, 0, MaxHP); // Ensure health stays within bounds
        }
    }

    private void ResetHP()
    {
        CurrentHP = MaxHP;
    }

    private void UpdateHealthSlider()
    {
        if (healthSlider != null)
        {
            healthSlider.value = CurrentHP;
        }
    }

    private void NoHealthPlayer()
    {
        if (CurrentHP <= 0)
        {
            Debug.Log("Game Over!");
            Application.Quit();
        }
    }
}
