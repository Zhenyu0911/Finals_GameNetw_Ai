using UnityEngine;
using TMPro;
using UnityEngine.UI;  // For Image component

public class PlayerStatusUI : MonoBehaviour
{
    public TMP_Text playerNameText;  // Reference to the player's name text
    public TMP_Text readyStatusText; // Reference to the player's ready status text
    public Image playerImage;        // Reference to the player's character image

    // This method will be called to update the player's name, status, and index
    public void SetPlayerInfo(string playerName, bool isReady, Sprite characterSprite, int playerIndex)
    {
        playerNameText.text = playerName;  // Set the player's name
        readyStatusText.text = isReady ? "Ready" : "Not Ready";  // Set the ready status
        playerImage.sprite = characterSprite;  // Set the character sprite
    }
}
