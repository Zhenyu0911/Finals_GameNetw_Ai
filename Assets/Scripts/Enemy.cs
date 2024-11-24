
using Photon.Pun;
using UnityEngine;

public class Enemy : MonoBehaviourPun
{
    public int health = 150;

    public void TakeTurn()
    {
        photonView.RPC("AttackPlayer", RpcTarget.All);
    }

    [PunRPC]
    void AttackPlayer()
    {
        Debug.Log("Enemy attacks!");
        // Implement logic to choose a random player to attack
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy defeated!");
        PhotonNetwork.Destroy(gameObject);
    }
}
