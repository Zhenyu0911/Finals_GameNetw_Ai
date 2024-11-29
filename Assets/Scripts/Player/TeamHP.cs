using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamHP : MonoBehaviour
{
    [SerializeField] public float MaxHP = 100.0f;
    [SerializeField] public float CurrentHP;

    Enemy enemy;

    // Start is called before the first frame update
    void Start()
    {
        ResetHP();
    }

    void Update()
    {
        TakeDamageFromEnemy();
        NoHealthPlayer();
    }

    private void TakeDamageFromEnemy()
    {
        CurrentHP = enemy.EnemyAttack() - CurrentHP;
    }

    private void ResetHP()
    {
        CurrentHP = MaxHP;
    }

    //sorry pero kupal kamign coders
    private void NoHealthPlayer()
    {
        if(CurrentHP <= 0)
        {
            Application.Quit();
        }
    }
}