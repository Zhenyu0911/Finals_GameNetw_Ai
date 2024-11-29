using Photon.Pun;
using UnityEngine;

public class ActionLogger : MonoBehaviourPun
{
    [SerializeField] private UIManager uiManager;

    [PunRPC]
    public void ReceiveAction(string actionMessage)
    {
        uiManager.LogAction(actionMessage);
    }
}
