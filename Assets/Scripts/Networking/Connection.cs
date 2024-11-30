using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class Connection : MonoBehaviourPunCallbacks
{
    private const int MAX_PLAYERS = 4;
    [SerializeField] private RectTransform connectingPanel;
    [SerializeField] private RectTransform connectedPanel;
    [SerializeField] private TextMeshProUGUI roomInfoText;

    private const string KEY_HOLDER = "KeyHolder"; // Custom property for the player holding the key

    public override void OnConnectedToMaster()
    {
        Debug.Log("We are connected");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        connectingPanel.gameObject.SetActive(false);
        connectedPanel.gameObject.SetActive(true);

        // Check if we're the first player to join
        if (PhotonNetwork.IsMasterClient && !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(KEY_HOLDER))
        {
            // Assign the key to the first player
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                { KEY_HOLDER, PhotonNetwork.LocalPlayer.ActorNumber }
            });
        }

        UpdateRoomInfo();
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} joined the room");
        UpdateRoomInfo();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} left the room");

        // Check if the player who left was holding the key
        int currentKeyHolder = (int)PhotonNetwork.CurrentRoom.CustomProperties[KEY_HOLDER];
        if (otherPlayer.ActorNumber == currentKeyHolder)
        {
            Debug.Log($"{otherPlayer.NickName} had the key. Reassigning...");

            // Reassign the key to the next player
            AssignKeyToNextPlayer();
        }

        UpdateRoomInfo();
    }

    private void AssignKeyToNextPlayer()
    {
        // Find the next available player in the room
        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if (player.IsInactive) continue; // Skip inactive players
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                { KEY_HOLDER, player.ActorNumber }
            });
            Debug.Log($"Key reassigned to {player.NickName} (ActorNumber: {player.ActorNumber})");
            break;
        }
    }

    private void UpdateRoomInfo()
    {
        Room currentRoom = PhotonNetwork.CurrentRoom;

        if (currentRoom == null) return;

        string formattedText = $"Room Name: {currentRoom.Name}<br>";
        int keyHolder = currentRoom.CustomProperties.ContainsKey(KEY_HOLDER)
            ? (int)currentRoom.CustomProperties[KEY_HOLDER]
            : -1;

        foreach (var player in currentRoom.Players)
        {
            if (player.Key == keyHolder)
            {
                formattedText += $"<color=#ee7358>[{player.Key}]: {player.Value.NickName} (Key Holder)</color><br>";
            }
            else
            {
                formattedText += $"[{player.Key}]: {player.Value.NickName}<br>";
            }
        }

        roomInfoText.text = formattedText;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"Failed to join a room: {message}");
        Debug.Log("Creating a new room...");

        PhotonNetwork.CreateRoom(
            roomName: $"{PhotonNetwork.NickName}'s Room",
            new RoomOptions
            {
                MaxPlayers = MAX_PLAYERS,
                CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
                {
                    { KEY_HOLDER, -1 } // Initialize without a key holder
                }
            });
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Already connected");
            return;
        }

        PhotonNetwork.ConnectUsingSettings();
    }
}
