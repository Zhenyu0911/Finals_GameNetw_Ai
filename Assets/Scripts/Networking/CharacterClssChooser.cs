using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections.Generic;

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

    private const string ClassPropertyKey = "PlayerClass"; // Key for class in custom properties

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
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        {
            { ClassPropertyKey, classIndex }
        });

        // Update the local UI immediately
        UpdateClassUI(classIndex);

        // Refresh class availability for everyone
        RefreshClassAvailability();
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
        // Get the player's current class property and update the UI
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(ClassPropertyKey, out object classIndexObj) && classIndexObj is int classIndex)
        {
            UpdateClassUI(classIndex);
            classDropdown.value = classIndex; // Synchronize the dropdown
        }
        else
        {
            // Default to the first class if no property is set
            OnClassSelected(0);
        }
    }

    private int GetCurrentClassIndex()
    {
        // Retrieve the player's current class index
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(ClassPropertyKey, out object classIndexObj) && classIndexObj is int classIndex)
        {
            return classIndex;
        }

        return -1; // Default value if no class is set
    }

    private bool IsClassAvailable(int classIndex)
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue(ClassPropertyKey, out object classIndexObj) && classIndexObj is int otherClassIndex)
            {
                if (otherClassIndex == classIndex)
                {
                    return false; // Class is taken
                }
            }
        }

        return true; // Class is available
    }

    private void RefreshClassAvailability()
    {
        // Refresh the dropdown options based on available classes
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        for (int i = 0; i < classNames.Count; i++)
        {
            bool available = IsClassAvailable(i);

            // Update dropdown options to indicate availability
            options.Add(new TMP_Dropdown.OptionData
            {
                text = available ? classNames[i] : $"{classNames[i]} (Taken)"
            });
        }

        classDropdown.options = options;
        classDropdown.RefreshShownValue();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey(ClassPropertyKey))
        {
            // Refresh class availability whenever a player updates their class
            RefreshClassAvailability();

            // Log for debugging
            if (changedProps[ClassPropertyKey] is int classIndex)
            {
                Debug.Log($"Player {targetPlayer.NickName} chose class: {classNames[classIndex]}");
            }
        }
    }
}
