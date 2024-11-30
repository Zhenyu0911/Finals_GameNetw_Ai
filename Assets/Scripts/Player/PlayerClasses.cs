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

public class PlayerClasses : MonoBehaviourPunCallbacks
{
    [SerializeField] public string Name; //Name of the class
    [SerializeField] public float Damage; //Base damage
    [SerializeField] public float CritRate; //critrate obv
    [SerializeField] public float CritDMG; //critdmg obv

    TeamHP healthTeam;

    public float randomCrit = Random.Range(1, 100); //a randomizer that act as a critrate


    public float PlayerDamage()
    {
        //if random number is between 1 - n, guaranteed hit
        //if (!alreadyAttacked) //checks if the player has not attacked
        {
            if (randomCrit > CritRate)
            {
                Debug.Log("CritDMG");
                Damage = Damage * CritDMG; //Guaranteed CritDMG
                //damage collision to playerblah blah
                healthTeam.CurrentHP =- Damage;
                return Damage;
            }

            else
            {
                Debug.Log("Normal DMG");  //guaranteed normalDMG
                //damage collision to playerblah blah
                healthTeam.CurrentHP =- Damage;
                return Damage;
            }
        }
    }

    //resets when after every round
    /*private void ResetAttack()
    {
        alreadyAttacked = false;
    }*/
}
