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

    public override void OnConnectedToMaster()
    {
       Debug.Log("We are connected");
       // The master server handles all the room information
       PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        connectingPanel.gameObject.SetActive(false);
        connectedPanel.gameObject.SetActive(true);

        UpdateRoomInfo();
        // Since it's a multiplayer, best to let Photon handle scene loading
        // Load Game Scene: 1
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
        UpdateRoomInfo();
    }


    private void UpdateRoomInfo(){
        Room currentRoom = PhotonNetwork.CurrentRoom;
        string formattedText =  $"Room Name: {currentRoom.Name}<br>";
            foreach(var player in currentRoom.Players){
                if(PhotonNetwork.IsMasterClient){
                     formattedText += $"<color=#ee7358>[{player.Key}]: {player.Value.NickName}</color><br>";
                }
                else{
                     formattedText += $"[{player.Key}]: {player.Value.NickName}<br>";
                }
            }
            roomInfoText.text = formattedText;
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"Failed to join a room: {message}");
        Debug.Log($"Create a room instead..");
        // If no rooms are available (all rooms are full or there are no rooms at all)..
        // Create own room
        PhotonNetwork.CreateRoom(
            roomName: $"{PhotonNetwork.NickName}'s Room",
            roomOptions: new RoomOptions{
                MaxPlayers = MAX_PLAYERS
            });
    }

    public void Connect()
    {
        if(PhotonNetwork.IsConnected){
            Debug.Log("Already connected");
            return;
        }

        // Connect to photon network
        PhotonNetwork.ConnectUsingSettings();
    }
}
