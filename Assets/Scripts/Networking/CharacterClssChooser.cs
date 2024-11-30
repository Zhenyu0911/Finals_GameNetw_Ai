using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class CharacterClassChooser : MonoBehaviourPunCallbacks
{
    [Header("Class Settings")]
    [SerializeField] private List<Sprite> classImages; // List of images for classes
    [SerializeField] private List<string> classNames;  // List of class names (ensure this matches classImages order)
    [SerializeField] private List<string> classDescriptions; // List of class descriptions

    [Header("UI Elements")]
    [SerializeField] private Image classImageUI;       // Image to display the current class
    [SerializeField] private TMP_Dropdown classDropdown; // TMP Dropdown for selecting classes
    [SerializeField] private TMP_Text classDescriptionText; // Text to display the current class description
    [SerializeField] private TMP_Text playerClassListText; // Text to display all players' selected classes
    [SerializeField] private Button startGameButton;    // Button for starting the game

    private const string ClassPropertyKey = "PlayerClass"; // Key for class in custom properties
    private const byte StartGameEventCode = 1;            // Custom event code for starting the game
    private const byte ClassChangeEventCode = 2;          // Custom event code for class change event

    private void Start()
    {
        // Populate the dropdown options with class names
        classDropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (var className in classNames)
        {
            options.Add(new TMP_Dropdown.OptionData(className));
        }
        classDropdown.AddOptions(options);

        // Add a listener for dropdown value changes
        classDropdown.onValueChanged.AddListener(OnClassSelected);

        // Update UI based on the current class property
        UpdateUIFromProperties();

        // Refresh available classes
        RefreshClassAvailability();

        // Update the class list UI
        UpdatePlayerClassList();

        // Set up the Start Game button
        if (startGameButton != null)
        {
            startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient); // Only show to the Master Client
            startGameButton.onClick.AddListener(StartGame);
        }
    }

    private void OnClassSelected(int classIndex)
    {
        if (classIndex < 0 || classIndex >= classImages.Count) return;

        // Check if the chosen class is already taken
        if (!IsClassAvailable(classIndex))
        {
            Debug.LogWarning($"Class {classNames[classIndex]} is already taken!");
            classDropdown.value = GetCurrentClassIndex(); // Revert to the current class
            return;
        }

        // Set the class property for the local player
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "PlayerClass", classIndex } });


        // Update the local UI immediately
        UpdateClassUI(classIndex);

        // Send class change event to notify all other players
        SendClassChangeEvent(classIndex);

        // Refresh class availability for everyone
        RefreshClassAvailability();

        // Update the class list UI
        UpdatePlayerClassList();
    }

    private void UpdateClassUI(int classIndex)
    {
        // Change the displayed image to match the chosen class
        if (classImageUI != null && classIndex >= 0 && classIndex < classImages.Count)
        {
            classImageUI.sprite = classImages[classIndex];
        }

        // Update the class description text
        if (classDescriptionText != null && classIndex >= 0 && classIndex < classDescriptions.Count)
        {
            classDescriptionText.text = classDescriptions[classIndex];
        }

        Debug.Log($"Player {PhotonNetwork.LocalPlayer.NickName} chose class: {classNames[classIndex]}");
    }

    private void UpdateUIFromProperties()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(ClassPropertyKey, out object classIndexObj) && classIndexObj is int classIndex)
        {
            UpdateClassUI(classIndex);
            classDropdown.value = classIndex; // Synchronize the dropdown
        }
        else
        {
            OnClassSelected(0); // Default to the first class if no class is selected yet
        }
    }

    private int GetCurrentClassIndex()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(ClassPropertyKey, out object classIndexObj) && classIndexObj is int classIndex)
        {
            return classIndex;
        }

        return -1; // Default value when no class is selected
    }

    private bool IsClassAvailable(int classIndex)
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue(ClassPropertyKey, out object classIndexObj) && classIndexObj is int otherClassIndex)
            {
                if (otherClassIndex == classIndex)
                {
                    return false; // Class is taken by another player
                }
            }
        }

        return true; // Class is available
    }

    private void RefreshClassAvailability()
    {
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        for (int i = 0; i < classNames.Count; i++)
        {
            bool available = IsClassAvailable(i);
            options.Add(new TMP_Dropdown.OptionData
            {
                text = available ? classNames[i] : $"{classNames[i]} (Taken)"
            });
        }

        classDropdown.options = options;
        classDropdown.RefreshShownValue();
    }

    private void UpdatePlayerClassList()
    {
        if (playerClassListText == null) return;

        string playerClasses = "Player Classes:\n";

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            string className = "None";

            if (player.CustomProperties.TryGetValue(ClassPropertyKey, out object classIndexObj) && classIndexObj is int classIndex)
            {
                className = classIndex >= 0 && classIndex < classNames.Count ? classNames[classIndex] : "Unknown";
            }

            playerClasses += $"{player.NickName}: {className}\n";
        }

        playerClassListText.text = playerClasses;
    }

    private void SendClassChangeEvent(int classIndex)
    {
        object[] content = new object[] { PhotonNetwork.LocalPlayer.NickName, classIndex };
        PhotonNetwork.RaiseEvent(ClassChangeEventCode, content, new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All
        }, new SendOptions { Reliability = true });
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        // Check if the PlayerClass property has changed
        if (changedProps.ContainsKey(ClassPropertyKey))
        {
            // Refresh class availability for everyone
            RefreshClassAvailability();

            // Update the class list UI for all players
            UpdatePlayerClassList();

            // Update the class UI if it's the local player's properties that changed
            if (targetPlayer == PhotonNetwork.LocalPlayer)
            {
                if (changedProps[ClassPropertyKey] is int classIndex)
                {
                    UpdateClassUI(classIndex);
                    classDropdown.value = classIndex; // Sync the dropdown
                }
            }
        }
    }

    public override void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public override void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == ClassChangeEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            string playerName = (string)data[0];
            int classIndex = (int)data[1];

            // Update the UI of other players when a class change is received
            if (PhotonNetwork.LocalPlayer.NickName != playerName)
            {
                Debug.Log($"Player {playerName} changed their class to {classNames[classIndex]}");

                // Update the other player's class in the list and UI
                UpdatePlayerClassList();
            }
        }

        if (photonEvent.Code == StartGameEventCode)
        {
            Debug.Log("All players received the Start Game event.");
            PhotonNetwork.LoadLevel("Fight"); // Replace with the name of your actual game scene
        }
    }

    private void StartGame()
    {
        // Send Start Game event to all players
        PhotonNetwork.RaiseEvent(StartGameEventCode, null, new RaiseEventOptions { Receivers = ReceiverGroup.All }, new SendOptions { Reliability = true });
    }
}
