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

    private Player player;

    private void Start()
    {
        // Default to a waiting state
        ShowConnectionUI(false);
        PlayerNumbering.OnPlayerNumberingChanged += UpdateUI;
    }

    // Add/remove the callback target so we can receive Photon raise events
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
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

        }
        else
        {
            // Player disconnected
            ShowConnectionUI(false);
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
}
