using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInputField : MonoBehaviour
{
    [SerializeField]
    private Button connectButton;

    public void SetPlayerName(string value){
        if(string.IsNullOrEmpty(value)){
           connectButton.interactable = false;
        }
        connectButton.interactable = true;
        //Set the name of the player in photon
        PhotonNetwork.NickName = value;
    }
}
