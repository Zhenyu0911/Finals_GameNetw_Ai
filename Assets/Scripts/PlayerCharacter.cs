
using Photon.Pun;
using UnityEngine;

public class PlayerCharacter : MonoBehaviourPun
{
    public string playerClass;
    public int health = 100;

    void Start()
    {
        if (photonView.IsMine)
        {
            var props = PhotonNetwork.LocalPlayer.CustomProperties;
            if (props.ContainsKey("Class"))
            {
                playerClass = (string)props["Class"];
            }
        }
    }

    public void PerformAction(string action)
    {
        if (action == "Attack")
        {
            photonView.RPC("AttackEnemy", RpcTarget.All);
        }
        else if (action == "Heal")
        {
            Heal(20);
        }
    }

    [PunRPC]
    void AttackEnemy()
    {
        Debug.Log($"{playerClass} attacks the enemy!");
        // Implement enemy damage logic here
    }

    void Heal(int amount)
    {
        health = Mathf.Min(health + amount, 100);
        Debug.Log($"{playerClass} heals for {amount} HP!");
    }
}
