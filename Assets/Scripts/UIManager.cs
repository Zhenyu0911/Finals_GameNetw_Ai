
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Slider playerHealthBar;
    public Slider enemyHealthBar;

    public void UpdateHealth(Slider healthBar, int currentHealth, int maxHealth)
    {
        healthBar.value = (float)currentHealth / maxHealth;
    }

    public void SetTurnIndicator(string message)
    {
        Debug.Log(message);
    }
}
