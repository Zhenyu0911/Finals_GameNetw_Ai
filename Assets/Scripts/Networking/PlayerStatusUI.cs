using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour, IOnEventCallback
{
    [SerializeField]
    private int playerIndex;

    [SerializeField]
    private GameObject connected, waiting;

    [SerializeField]
    private TextMeshProUGUI playerName;

    [SerializeField]
    private Button startGameButton; // Reference to the start button

    private Player player;

    private void Start()
    {
        // Default to a waiting state
        ShowConnectionUI(false);

        if (startGameButton != null)
        {
            startGameButton.interactable = false; // Disable the button by default
            startGameButton.onClick.AddListener(StartGame); // Attach the start game logic
        }

        PlayerNumbering.OnPlayerNumberingChanged += UpdateUI;
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
        PlayerNumbering.OnPlayerNumberingChanged -= UpdateUI;
    }

    private void UpdateUI()
    {
        // Check if there is an available player with our index
        if (playerIndex <= PhotonNetwork.PlayerList.Length - 1)
        {
            player = PhotonNetwork.PlayerList[playerIndex];
            int playerNumber = player.GetPlayerNumber();

            // Add a check if the player number is valid
            if (playerNumber == -1) return;

            ShowConnectionUI(true);
            playerName.text = player.NickName;

            // Enable the start game button only for the local player if they are the Master Client
            if (PhotonNetwork.IsMasterClient && playerIndex == 0 && startGameButton != null)
            {
                startGameButton.interactable = true;
            }
        }
        else
        {
            // Player disconnected
            ShowConnectionUI(false);

            if (startGameButton != null)
            {
                startGameButton.interactable = false;
            }
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        // Handle Photon events if necessary
    }

    private void ShowConnectionUI(bool isConnected)
    {
        waiting.SetActive(!isConnected);
        connected.SetActive(isConnected);
    }

    private void StartGame()
    {
        // Ensure only the Master Client can start the game
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.LocalPlayer == PhotonNetwork.PlayerList[0])
        {
            Debug.Log("Game started by the lobby owner!");

            // Raise an event to notify all players to load the next scene
            PhotonNetwork.RaiseEvent(1, null, new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All // Ensure all players receive this event
            }, new SendOptions { Reliability = true });
        }
        else
        {
            Debug.LogWarning("Only the lobby owner (Master Client) can start the game!");
        }
    }
}
