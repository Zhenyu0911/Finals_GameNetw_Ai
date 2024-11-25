using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;


public class PlayerReadyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button readyButton;         // Button for toggling readiness
    [SerializeField] private Image buttonImage;         // Image component to change button color
    [SerializeField] private TextMeshProUGUI statusText; // Text to display "Ready" or "Unready"

    private const string ReadyPropertyKey = "IsReady"; // Key for ready state in custom properties
    private int playerID;                             // Unique ID for each player's ready status

    private void Start()
    {
        // Assign listener to the button
        if (readyButton != null)
        {
            readyButton.onClick.AddListener(ToggleReadyState);
        }

        // Set the player ID using the actor number (Photon's unique identifier for each player)
        playerID = PhotonNetwork.LocalPlayer.ActorNumber;

        // Update UI when starting based on the player's current custom properties
        UpdateUIFromProperties();
    }

    private void ToggleReadyState()
    {
        // Toggle the local player's ready state
        bool isReady = !GetReadyState();

        // Update only the local player's custom properties with their unique ID
        // Set the property for the playerID using their specific custom key
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        {
            { ReadyPropertyKey + playerID, isReady }
        });

        // Update local UI immediately
        UpdateUI(isReady);
    }

    private void UpdateUI(bool isReady)
    {
        // Change button color and status text
        if (buttonImage != null)
        {
            buttonImage.color = isReady ? Color.red : Color.green;
        }

        if (statusText != null)
        {
            statusText.text = isReady ? "Ready" : "Unready";
        }

        Debug.Log($"Player {PhotonNetwork.LocalPlayer.NickName} (ID {playerID}) is now {(isReady ? "Ready" : "Unready")}");
    }

    private void UpdateUIFromProperties()
    {
        // Retrieve the ready state from the local player's properties and update the UI
        bool isReady = GetReadyState();
        UpdateUI(isReady);
    }

    private bool GetReadyState()
    {
        // Retrieve the "IsReady" property value for the local player
        // This checks the specific property using the unique playerID to avoid syncing across players
        return PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(ReadyPropertyKey + playerID, out object isReady) && (bool)isReady;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        // Only update the UI for the local player when their own properties are updated
        if (changedProps.ContainsKey(ReadyPropertyKey + playerID))
        {
            // If the property belongs to this local player, update the UI
            if (targetPlayer == PhotonNetwork.LocalPlayer)
            {
                UpdateUIFromProperties();
            }

            // Log other players' state changes for visibility in debugging
            Debug.Log($"Player {targetPlayer.NickName} (ID {targetPlayer.ActorNumber}) is now {(changedProps[ReadyPropertyKey + playerID] is bool ready && ready ? "Ready" : "Unready")}");
        }
    }
}
