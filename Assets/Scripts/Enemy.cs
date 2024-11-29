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
    [SerializeField] public float MaxHP; //MaxHealth of the enemy
    [SerializeField] public float CurrentHP; //Set HP
    [SerializeField] public float Damage; //DamageEnemy
    [SerializeField] public float CritRate; //Rate of hitting
    [SerializeField] public float CritDMG; //increase rate of DMG
    public float randomCrit = Random.Range(0, 100);
    public bool alreadyAttacked;

    //take damage from player
    private void TakeDamageHP()
    {
        //if damage hit enemy blah blah blah
        //CurrentHP = //From playerGameObject
    }

    //Attack damage to player
    private void AttackDamage()
    {
            Debug.Log("Attacked");
            //damage collision to playerblah blah
            Damage = +CritDMG;
            CurrentHP = -Damage;
            alreadyAttacked = true;
    }

    //change of hitting the player
    private void DamageChance()
    {
        //if random number is between 1 - n, guaranteed hit
            if(!alreadyAttacked) //checks if the enemy has not attacked
        {
            if (randomCrit > CritRate)
            {
                Debug.Log("GuaranteedHit");
                AttackDamage();
            }

            else
            {
                Debug.Log("You missed");
            }
        }
    }

    //resets when after every round
    private void ResetAttack()
    {
        alreadyAttacked = false; 
    }

    //When collides with a player to damage
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DamageChance();
        }
    }

      
}
