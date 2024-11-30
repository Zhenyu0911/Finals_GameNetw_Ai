using Photon.Pun;
using Photon.Realtime; // Required for SendOptions and RaiseEventOptions
using System.Collections.Generic;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.UI;


public class NetworkManager : SingletonPUN<NetworkManager>
{
    // Events:
    public const byte SCORE_UPDATED_EVENT_CODE = 1;

    public bool IsInitialized = false;

    protected override void Awake()
    {
        base.Awake();
        if (!PhotonNetwork.IsConnected)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            return;
        }
    }
}
