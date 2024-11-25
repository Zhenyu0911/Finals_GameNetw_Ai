using UnityEngine;

public class ManagingScript : MonoBehaviour
{
    public static ManagingScript Instance;

    private void Awake()
    {
        // Ensure only one instance of ManagingScript exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // This method will be called when the planet's health reaches zero
    public void ShowGameOver()
    {
        UnityEngine.Debug.Log("Game Over!"); // Use UnityEngine.Debug explicitly
        // Handle game over logic (show UI, etc.)
    }
}
