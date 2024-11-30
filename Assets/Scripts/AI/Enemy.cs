using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviourPunCallbacks
{
    [SerializeField] public float MaxHP; // MaxHealth of the Enemy
    [SerializeField] public float Damage; // Base Damage of the Enemy
    [SerializeField] public float CritRate; // Chance of a critical hit (percentage)
    [SerializeField] public float CritDMG; // Multiplier for critical damage

    PlayerClasses playerStats;

    private float CurrentHP; // Enemy's current health
    private float randomCrit; // Random number for critical hit calculation

    void Start()
    {
        ResetHP();

        // Initialize randomCrit in Start
        randomCrit = Random.Range(1, 101); // Generate a number between 1 and 100
    }

    void Update()
    {
        TakeDamageHP();
    }

    private void ResetHP()
    {
        CurrentHP = MaxHP;
    }

    // Reduce enemy health based on player stats
    private void TakeDamageHP()
    {
        if (playerStats != null)
        {
            CurrentHP -= playerStats.PlayerDamage();
        }
    }

    // Enemy attack logic with critical hit chance
    public float EnemyAttack()
    {
        if (randomCrit <= CritRate)
        {
            Debug.Log("Critical Damage!");
            return Damage * CritDMG; // Apply critical damage multiplier
        }
        else
        {
            Debug.Log("Normal Damage!");
            return Damage; // Return normal damage
        }
    }
}
