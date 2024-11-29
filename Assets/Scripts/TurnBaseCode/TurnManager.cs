using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TurnManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private UIManager uiManager;
    private const string TurnIndexPropertyKey = "TurnIndex";
    private int currentTurnIndex = 0;
    private int playerCount = 4;

    private bool enemyTurn = false;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                { TurnIndexPropertyKey, currentTurnIndex }
            });
        }

        UpdateTurnIndicator();
    }

    public void EndTurn()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            currentTurnIndex = (currentTurnIndex + 1) % playerCount;
            if (currentTurnIndex == playerCount - 1)
            {
                enemyTurn = true;
            }
            else
            {
                enemyTurn = false;
            }

            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                { TurnIndexPropertyKey, currentTurnIndex }
            });
        }
    }

    private void UpdateTurnIndicator()
    {
        if (enemyTurn)
        {
            uiManager.UpdateTurnIndicator("It's the enemy's turn!");
        }
        else
        {
            Player currentPlayer = PhotonNetwork.PlayerList[currentTurnIndex];
            uiManager.UpdateTurnIndicator($"It's {currentPlayer.NickName}'s turn!");
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.TryGetValue(TurnIndexPropertyKey, out object turnIndexObj) && turnIndexObj is int turnIndex)
        {
            currentTurnIndex = turnIndex;
            UpdateTurnIndicator();
        }
    }
}
