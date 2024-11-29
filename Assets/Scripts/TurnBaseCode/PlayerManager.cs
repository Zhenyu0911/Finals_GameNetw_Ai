using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    private bool isPlayerTurn = true;

    public void PerformAction(string actionType)
    {
        if (!isPlayerTurn)
        {
            Debug.LogWarning("It's not your turn!");
            return;
        }

        string actionMessage = $"Player performs {actionType}!";
        uiManager.UpdateActionFeed(actionMessage);

        if (actionType == "Attack")
        {
            DealDamage(10);
        }

        EndPlayerTurn();
    }

    private void DealDamage(int damage)
    {
        SharedHealthSystem.Instance.TakeDamage(damage);
    }

    private void EndPlayerTurn()
    {
        isPlayerTurn = false;
        uiManager.UpdateActionButtons(false);
        FindObjectOfType<TurnManager>().EndTurn();
    }

    public void StartPlayerTurn()
    {
        isPlayerTurn = true;
        uiManager.UpdateActionButtons(true);
    }
}
