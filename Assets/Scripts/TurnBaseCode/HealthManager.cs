using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [Header("Health UI Elements")]
    [SerializeField] private Slider healthSlider; // Reference to the health slider UI
    [SerializeField] private int maxHealth = 100; // Max health value

    public int CurrentHealth { get; private set; } // Current health value

    private void Start()
    {
        // Initialize current health
        CurrentHealth = maxHealth;
        healthSlider.maxValue = maxHealth; // Set the max value of the slider
        healthSlider.value = CurrentHealth; // Set the initial value
    }

    public void UpdateHealth(int newHealth)
    {
        CurrentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        healthSlider.value = CurrentHealth;

        // Optionally, update room properties or notify other players
        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Health", CurrentHealth } });
    }
}
