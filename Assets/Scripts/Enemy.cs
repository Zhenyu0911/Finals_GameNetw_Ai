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
    [SerializeField] public float MaxHP = 100.0f; //MaxHealth of the enemy
    [SerializeField] public int CurrentHP; //Set HP
    [SerializeField] public float Damage = 20.0f; //DamageEnemy
    [SerializeField] public float CritRate = 1.2f; //increase of rate in hitting
    [SerializeField] public float CritDMG; //increase rate of DMG
    [SerializeField] public float HitChance = 6.0f; //default setting of chance (60/40)

    void HitChance
    {

    }

    void TakeDamage
    {

    }

    void AttackDamage
    {

    }

    void RandomCritRate
    {

    }

    void RandomCritDamage
    {

    }
      
}
