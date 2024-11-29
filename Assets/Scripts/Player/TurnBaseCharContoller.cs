using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class TurnBasedActionSystem : MonoBehaviourPunCallbacks
{
    [Header("Class Settings")]
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
    }

    private GameObject FindPlayerObject(Player player)
    {
        return GameObject.Find($"Player_{player.NickName}");
    }

    private void UpdateTurnIndicator()
    {
        if (turnIndicatorText != null)
        {
            Player currentPlayer = PhotonNetwork.PlayerList[currentTurnIndex];
            if (enemyTurn)
            {
                turnIndicatorText.text = $"It's the enemy's turn!";
            }
            else
            {
                turnIndicatorText.text = $"It's {currentPlayer.NickName}'s turn!";
            }
        }
    }

    private void UpdateActionButtons()
    {
        bool isCurrentPlayer = PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.PlayerList[currentTurnIndex].ActorNumber;

        attackButton.interactable = isCurrentPlayer && isPlayerTurn && !enemyTurn;
        abilityButton.interactable = isCurrentPlayer && isPlayerTurn && !enemyTurn;
        endTurnButton.interactable = isCurrentPlayer && isPlayerTurn && !enemyTurn;
    }

    private void LogAction(string message)
    {
        if (actionFeedText != null)
        {
            actionFeedText.text = message;
            photonView.RPC(nameof(ReceiveAction), RpcTarget.All, message);
        }
    }

    public void PerformAction(string actionType)
    {
        if (!isPlayerTurn)
        {
            Debug.LogWarning("It's not your turn or you already performed an action!");
            return;
        }

        Player currentPlayer = PhotonNetwork.PlayerList[currentTurnIndex];

        if (PhotonNetwork.LocalPlayer == currentPlayer)
        {
            string actionMessage = $"{currentPlayer.NickName} uses {actionType}!";
            LogAction(actionMessage);

            // Disable further actions for this player
            isPlayerTurn = false;

            // Broadcast action to all players
            photonView.RPC(nameof(ReceiveAction), RpcTarget.All, actionMessage);

            // End the current player's turn
            EndTurn();
        }
        else
        {
            Debug.LogWarning("It's not your turn!");
        }
    }

    private void EndTurn()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (!enemyTurn)
            {
                currentTurnIndex = (currentTurnIndex + 1) % (playerCount - 1); // Cycle through players
                isPlayerTurn = true;
            }
            else
            {
                // After all players' turns, it's the enemy's turn
                currentTurnIndex = playerCount - 1; // The last slot for the enemy
                enemyTurn = true; // It's now the enemy's turn
            }

            // Update the turn index in room properties
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                { TurnIndexPropertyKey, currentTurnIndex }
            });
        }
    }

    private void EndRound()
    {
        // After all players have taken their turn, let the enemy perform their action
        if (PhotonNetwork.IsMasterClient)
        {
            // Here you would have the logic for the enemy's action (attacks, abilities, etc.)
            string actionMessage = "Enemy performs an attack!";
            LogAction(actionMessage);

            // End enemy's turn and cycle back to players
            enemyTurn = false;
            currentTurnIndex = 0; // Go back to Player 1 (or the master client)
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                { TurnIndexPropertyKey, currentTurnIndex }
            });
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.TryGetValue(TurnIndexPropertyKey, out object turnIndexObj) && turnIndexObj is int turnIndex)
        {
            currentTurnIndex = turnIndex;
            UpdateTurnIndicator();
            UpdateActionButtons();

            // After all players have taken their turn, trigger enemy action
            if (currentTurnIndex == playerCount - 1 && !enemyTurn)
            {
                EndRound();
            }
        }
    }

    [PunRPC]
    private void ReceiveAction(string actionMessage)
    {
        LogAction(actionMessage);
    }
}
