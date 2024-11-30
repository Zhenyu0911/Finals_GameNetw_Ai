using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun.Demo.Asteroids;
using Unity.VisualScripting;

public class Enemy : MonoBehaviourPunCallbacks
{
    [SerializeField] public float MaxHP; //MaxHealth of the
    [SerializeField] public float Damage; //DamageEnemy
    [SerializeField] public float CritRate; //Rate of hitting
    [SerializeField] public float CritDMG; //increase rate of DMG

    PlayerClasses playerStats;

    private float CurrentHP; //Set HP
    private float randomCrit = Random.Range(1, 100);

    void Start()
    {
        ResetHP();
    }

    void Update()
    {
        TakeDamageHP();
    }

    private void ResetHP()
    {
        CurrentHP = MaxHP;
    }
    //take damage from player
    private void TakeDamageHP()
    {
        CurrentHP = playerStats.PlayerDamage() - CurrentHP;
    }

        //change of hitting the player
    public float EnemyAttack()
    {
        //if random number is between 1 - n, guaranteed hit
        //if (!alreadyAttacked) //checks if the enemy has not attacked
        //{
            if (randomCrit > CritRate)
            {
                Debug.Log("CritDMG");
                Damage = Damage * CritDMG;
                return Damage;
            }

            else
            {
                Debug.Log("Normal DMG");
                return Damage;
            }
       // }
        //else
        //{
           // Debug.Log("Already Attacked!");
           // return 0;
        //}
    }

    //resets when after every round
    /*private void ResetAttack()
    {
        alreadyAttacked = false; 
    }*/

         
}
