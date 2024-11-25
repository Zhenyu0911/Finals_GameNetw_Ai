using Photon.Pun;
using Photon.Realtime; // Required for SendOptions and RaiseEventOptions
using System.Collections.Generic;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.UI;


public class NetworkManager : SingletonPUN<NetworkManager>
{
    // Events:
    public const byte SCORE_UPDATED_EVENT_CODE = 1;

    private const string PLAYER_PREFAB_NAME = "Player";
    [SerializeField]
    private Sprite[] playerIcons;

    public bool IsInitialized = false;

    private Dictionary<int, bool> playerReadyStates = new Dictionary<int, bool>(); // Player ready states

    [SerializeField] private Button readyButton; // Assign your Ready button in the Inspector
    [SerializeField] private Image buttonImage; // Assign the button's image for color changes

    protected override void Awake()
    {
        base.Awake();
        if (!PhotonNetwork.IsConnected)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            return;
        }

        // Spawn the player
        GameObject player = PhotonNetwork.Instantiate(PLAYER_PREFAB_NAME, Vector3.zero, Quaternion.identity);
        IsInitialized = true;

        // Add listener to the ready button
        if (readyButton != null)
        {
            readyButton.onClick.AddListener(ToggleReadyState);
        }
    }

    public Sprite GetPlayerIcon(int id)
    {
        if (id > -1 && id < playerIcons.Length)
        {
            return playerIcons[id];
        }
        else
        {
            Debug.LogWarning($"Cannot access sprite with id {id}");
        }
        return null;
    }

    private void ToggleReadyState()
    {
        int playerId = PhotonNetwork.LocalPlayer.ActorNumber;

        // Toggle the ready state
        bool isReady = playerReadyStates.ContainsKey(playerId) && playerReadyStates[playerId];
        playerReadyStates[playerId] = !isReady;

        // Update button color and log debug message
        if (buttonImage != null)
        {
            buttonImage.color = playerReadyStates[playerId] ? Color.green : Color.red;
        }

        if (playerReadyStates[playerId])
        {
            Debug.Log($"{PhotonNetwork.LocalPlayer.NickName} is Ready.");
        }
        else
        {
            Debug.Log($"{PhotonNetwork.LocalPlayer.NickName} is Unready.");
        }

        // Optionally, you can broadcast the ready state to other players using Photon
       // RaiseReadyStateEvent(playerReadyStates[playerId]);
    }

    /*private void RaiseReadyStateEvent(bool isReady)
    {
        // Send a custom event to notify other players about the ready state
        object[] content = new object[] { PhotonNetwork.LocalPlayer.ActorNumber, isReady }; // Custom content
        PhotonNetwork.RaiseEvent(SCORE_UPDATED_EVENT_CODE, content,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            SendOptions.SendReliable);
    }*/
}
