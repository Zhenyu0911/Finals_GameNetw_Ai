using UnityEngine;

public class SharedHealthSystem : MonoBehaviour
{
    public static SharedHealthSystem Instance { get; private set; }

    private int maxHealth = 100;
    private int currentHealth;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Debug.Log("Game Over!");
        }
    }
}
