using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class TurnBasedActionSystem : MonoBehaviourPunCallbacks
{
    /*[Header("Class Settings")]
    [SerializeField] private List<Sprite> classSprites; // List of sprites for each class
    [SerializeField] private List<string> classNames;   // Class names corresponding to sprites
    [SerializeField] private List<string> abilities;    // Abilities corresponding to classes

    [Header("UI Elements")]
    [SerializeField] private TMP_Text turnIndicatorText; // Text to display whose turn it is
    [SerializeField] private TMP_Text actionFeedText;    // Text to display the action feed
    [SerializeField] private Button attackButton;
    [SerializeField] private Button abilityButton;
    [SerializeField] private Button endTurnButton;

    private const string TurnIndexPropertyKey = "TurnIndex"; // Key for tracking current turn
    private const string PlayerSpriteKey = "PlayerSprite";  // Key for player sprite index

    private int currentTurnIndex = 0; // Local index of the current turn
    private bool isPlayerTurn = true; // To track if it's a player's turn
    private int playerCount = 4; // Total number of players (3 players + 1 enemy)
    private bool enemyTurn = false; // Flag to track enemy's turn
    public Slider healthSlider; // Reference to the health slider UI
    private int maxHealth = 100; // Max health value
    private int currentHealth; // Current health value

    private void Start()
    {
        AssignSpritesToPlayers();
        UpdateTurnIndicator();
        UpdateActionButtons();

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                { TurnIndexPropertyKey, currentTurnIndex }
            });
        }
        // Initialize current health
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth; // Set the max value of the slider
        healthSlider.value = currentHealth; // Set the initial value
        // Add listeners to buttons
        attackButton.onClick.AddListener(() => PerformAction("Attack"));
        abilityButton.onClick.AddListener(() => PerformAction("Special Ability"));
        endTurnButton.onClick.AddListener(EndTurn);
    }


    private void AssignSpritesToPlayers()
    {
        int assignedSpriteIndex = 0;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!player.CustomProperties.ContainsKey(PlayerSpriteKey))
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
                {
                    { PlayerSpriteKey, assignedSpriteIndex }
                });
                assignedSpriteIndex++;
            }
        }

        UpdatePlayerSprites();
    }

    private void UpdatePlayerSprites()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue(PlayerSpriteKey, out object spriteIndexObj) && spriteIndexObj is int spriteIndex)
            {
                if (spriteIndex >= 0 && spriteIndex < classSprites.Count)
                {
                    GameObject playerObject = FindPlayerObject(player);
                    if (playerObject != null)
                    {
                        SpriteRenderer renderer = playerObject.GetComponent<SpriteRenderer>();
                        if (renderer != null)
                        {
                            renderer.sprite = classSprites[spriteIndex];
                        }
                    }
                }
            }
        }
    }*/

}    