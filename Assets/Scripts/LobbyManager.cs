using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField playerNameInput;  // Use TMP_InputField for player name input
    public Button findOrCreateLobbyButton;  // Use Button from UnityEngine.UI
    public GameObject lobbyPanel;
    public GameObject readyPanel;
    public Button readyButton;
    public Button startGameButton;
    public TMP_Text playerListText;

    // Reference for the player status UI prefab
    public GameObject playerStatusUIPrefab;  // The prefab for the player status UI
    public Transform playerStatusUIContainer;  // Container to hold all player status UI in the Ready Panel

    private bool isReady = false;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        lobbyPanel.SetActive(true);
        readyPanel.SetActive(false);
        startGameButton.gameObject.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        findOrCreateLobbyButton.interactable = true;
        UpdatePlayerList();
    }

    public void SetPlayerName()
    {
        string playerName = playerNameInput.text.Trim();
        if (!string.IsNullOrEmpty(playerName))
        {
            PhotonNetwork.NickName = playerName;
            Debug.Log($"Player name set to {PhotonNetwork.NickName}");

            lobbyPanel.SetActive(false);
            readyPanel.SetActive(true);
            UpdatePlayerList();
        }
        else
        {
            Debug.Log("Player name is empty. Please enter a valid name.");
        }
    }

    public void FindOrCreateLobby()
    {
        SetPlayerName();
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No lobbies found. Creating a new room.");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room!");
        lobbyPanel.SetActive(false);
        readyPanel.SetActive(true);
        UpdatePlayerList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} joined the room!");
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} left the room.");
        UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        // Clear existing player status UI before updating
        foreach (Transform child in playerStatusUIContainer)
        {
            Destroy(child.gameObject);
        }

        int playerIndex = 1;  // Start the index at 1

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            // Instantiate the player status UI prefab
            GameObject playerStatusUI = Instantiate(playerStatusUIPrefab, playerStatusUIContainer);

            // Set the player's information (name, ready status, sprite, index)
            TMP_Text playerNameText = playerStatusUI.GetComponentInChildren<TMP_Text>();
            bool isReady = player.CustomProperties.ContainsKey("Ready") && (bool)player.CustomProperties["Ready"];
            Sprite playerSprite = GetCharacterSpriteForPlayer(player);  // Example method to get the sprite
            playerStatusUI.GetComponent<PlayerStatusUI>().SetPlayerInfo(player.NickName, isReady, playerSprite, playerIndex);

            playerIndex++;  // Increment the index for the next player
        }
    }

    private Sprite GetCharacterSpriteForPlayer(Player player)
    {
        // Example: Assign a random sprite or predefined sprite based on player name
        Sprite defaultSprite = Resources.Load<Sprite>("DefaultCharacterSprite");  // Replace with your actual sprite
        return defaultSprite;
    }

    public void ReadyUp()
    {
        if (readyButton == null)
        {
            Debug.LogError("Ready Button is not assigned in the Inspector.");
            return;
        }

        isReady = !isReady;

        readyButton.GetComponentInChildren<TMP_Text>().text = isReady ? "Unready" : "Ready";

        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Ready", isReady } });

        CheckAllPlayersReady();

        Debug.Log($"Player is now {(isReady ? "Ready" : "Not Ready")}");
    }

    private void CheckAllPlayersReady()
    {
        bool allReady = true;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!player.CustomProperties.ContainsKey("Ready") || !(bool)player.CustomProperties["Ready"])
            {
                allReady = false;
                break;
            }
        }

        startGameButton.gameObject.SetActive(allReady && PhotonNetwork.IsMasterClient);
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
    }
}
