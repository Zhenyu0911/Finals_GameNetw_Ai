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

            // Enable the start game button only for playerIndex 0 and the local player
            if (PhotonNetwork.LocalPlayer == PhotonNetwork.PlayerList[0] && playerIndex == 0 && startGameButton != null)
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
        if (PhotonNetwork.LocalPlayer == PhotonNetwork.PlayerList[0] && playerIndex == 0)
        {
            Debug.Log("Game started by player 0!");
            // Add your game-starting logic here
            // Example: PhotonNetwork.LoadLevel("GameScene");
        }
        else
        {
            Debug.LogWarning("Only player with playerIndex 0 can start the game!");
        }
    }
}
