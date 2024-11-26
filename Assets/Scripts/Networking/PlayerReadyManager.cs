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

    private const string ReadyPropertyKey = "IsReady_"; // Base key for ready state in custom properties

    private int playerID; // Unique ID for each player's ready status

    private void Start()
    {
        if (readyButton != null)
        {
            readyButton.onClick.AddListener(ToggleReadyState);
        }

        playerID = PhotonNetwork.LocalPlayer.ActorNumber;

        UpdateUIFromProperties();
    }

    private void ToggleReadyState()
    {
        bool isReady = !GetReadyState();

        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        {
            { ReadyPropertyKey + playerID, isReady }
        });

        UpdateUI(isReady);
    }

    private void UpdateUI(bool isReady)
    {
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
        bool isReady = GetReadyState();
        UpdateUI(isReady);
    }

    private bool GetReadyState()
    {
        return PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(ReadyPropertyKey + playerID, out object isReady) && (bool)isReady;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        int targetPlayerID = targetPlayer.ActorNumber;
        string targetKey = ReadyPropertyKey + targetPlayerID;

        if (changedProps.ContainsKey(targetKey))
        {
            if (targetPlayer == PhotonNetwork.LocalPlayer)
            {
                // Update only the UI for the local player
                UpdateUIFromProperties();
            }

            Debug.Log($"Player {targetPlayer.NickName} (ID {targetPlayerID}) updated their ready state.");
        }
    }
}
