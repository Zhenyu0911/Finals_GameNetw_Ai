
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviourPun
{
    private Queue<int> turnQueue = new Queue<int>();
    public int CurrentTurnPlayerId { get; private set; }

    void Start()
    {
        InitializeTurnQueue();
        StartNextTurn();
    }

    private void InitializeTurnQueue()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            turnQueue.Enqueue(player.ActorNumber);
        }
    }

    public void StartNextTurn()
    {
        if (turnQueue.Count == 0) InitializeTurnQueue();

        CurrentTurnPlayerId = turnQueue.Dequeue();
        photonView.RPC("SyncTurn", RpcTarget.All, CurrentTurnPlayerId);
    }

    [PunRPC]
    void SyncTurn(int playerId)
    {
        Debug.Log($"It's Player {playerId}'s turn!");
        if (PhotonNetwork.LocalPlayer.ActorNumber == playerId)
        {
            Debug.Log("This is your turn. Perform an action.");
        }
    }

    public void EndTurn()
    {
        StartNextTurn();
    }
}
