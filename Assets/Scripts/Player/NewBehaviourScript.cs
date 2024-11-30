using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class TurnBasedSystem : MonoBehaviour
{
    [Header("Turn System UI")]
    [SerializeField] private TMP_Text turnFeedText;  // Text to display turn updates
    [SerializeField] private Button attackButton;   // Button for Attack
    [SerializeField] private Button abilityButton;  // Button for Ability

    [Header("Health Settings")]
    [SerializeField] private Slider playerHealthSlider; // Player health slider
    [SerializeField] private Slider enemyHealthSlider;  // Enemy health slider
    [SerializeField] private int playerMaxHP = 100;
    [SerializeField] private int enemyMaxHP = 100;

    [Header("Player and Enemy Damage")]
    [SerializeField] private int playerAttackDamage = 20;
    [SerializeField] private int playerAbilityDamage = 30;
    [SerializeField] private int enemyAttackDamage = 15;

    private int CritRate = 1;
    private int randomCrit;

    private int playerCurrentHP;
    private int enemyCurrentHP;

    private bool isPlayerTurn = true;
    private bool actionTaken = false;

    private PhotonView photonView;  // PhotonView reference for RPC

    void Start()
    {
        photonView = GetComponent<PhotonView>();  // Get the PhotonView component attached to this object

        // Initialize health values and sliders
        playerHealthSlider.maxValue = playerMaxHP;
        enemyHealthSlider.maxValue = enemyMaxHP;

        playerCurrentHP = playerMaxHP;
        enemyCurrentHP = enemyMaxHP;

        UpdateHealthSliders();

        // Assign button listeners
        attackButton.onClick.AddListener(() => PlayerAttack());
        abilityButton.onClick.AddListener(() => PlayerAbility());

        // Start the game loop
        StartCoroutine(GameLoop());
    }

    private void Update()
    {
        randomCrit = Random.Range(0, 2); // 0 or 1
    }

    private IEnumerator GameLoop()
    {
        while (playerCurrentHP > 0 && enemyCurrentHP > 0)
        {
            if (isPlayerTurn)
            {
                yield return StartCoroutine(PlayerTurn());
            }
            else
            {
                yield return StartCoroutine(EnemyTurn());
            }
        }

        // Check for game over
        if (playerCurrentHP <= 0)
        {
            turnFeedText.text = "Game Over! Enemy Wins!";
        }
        else if (enemyCurrentHP <= 0)
        {
            turnFeedText.text = "You Win! Enemy Defeated!";
        }

        // Disable buttons after the game ends
        attackButton.gameObject.SetActive(false);
        abilityButton.gameObject.SetActive(false);
    }

    private IEnumerator PlayerTurn()
    {
        actionTaken = false; // Reset action taken flag
        EnableButtons(true); // Enable buttons for player input
        turnFeedText.text = "Choose an action!";

        // Wait for the player to take an action
        while (!actionTaken)
        {
            yield return null;
        }

        // Simulate delay after player's action
        yield return new WaitForSeconds(0.5f);

        // Update health sliders to reflect changes
        UpdateHealthSliders();

        // End player's turn
        isPlayerTurn = false;
    }

    private IEnumerator EnemyTurn()
    {
        EnableButtons(false); // Disable buttons during enemy's turn
        turnFeedText.text = "Enemy's Turn!";
        yield return new WaitForSeconds(1f);

        // Enemy attacks
        turnFeedText.text = "Enemy attacks you!";
        playerCurrentHP -= enemyAttackDamage;

        // Ensure player HP doesn't go below 0
        playerCurrentHP = Mathf.Max(playerCurrentHP, 0);

        // Update player health and slider immediately
        UpdateHealthSliders();

        // Simulate delay for enemy attack
        yield return new WaitForSeconds(1f);

        // Update player's HP in the feed
        turnFeedText.text = $"Your HP: {playerCurrentHP}";

        // End enemy's turn
        isPlayerTurn = true;
        yield return new WaitForSeconds(1f);
    }

    private void PlayerAttack()
    {
        if (!isPlayerTurn || actionTaken) return; // Prevent multiple actions

        // Make sure turnFeedText is assigned before using it
        if (turnFeedText != null)
        {
            turnFeedText.text = "You attack the enemy!";
        }
        else
        {
            Debug.LogError("turnFeedText is not assigned!");
        }

        if (photonView != null)
        {
            // Send an RPC to perform the attack on the enemy
            photonView.RPC("RPC_PlayerAttack", RpcTarget.AllBuffered, playerAttackDamage);
        }
        else
        {
            Debug.LogError("photonView is not assigned!");
        }

        actionTaken = true;
    }

    private void PlayerAbility()
    {
        if (actionTaken) return; // Prevent multiple actions in one turn

        // Make sure turnFeedText is assigned before using it
        if (turnFeedText == null)
        {
            Debug.LogError("turnFeedText is not assigned!");
        }
        else
        {
            Debug.Log("Ability called");
            turnFeedText.text = "You attempt to use your ability!";
        }

        if (photonView != null)
        {
            // Send an RPC to perform the ability action on the enemy
            photonView.RPC("RPC_PlayerAbility", RpcTarget.AllBuffered, playerAbilityDamage, randomCrit);
        }
        else
        {
            Debug.LogError("photonView is not assigned!");
        }

        actionTaken = true;
    }

    // RPC method to synchronize attack action across all players
    [PunRPC]
    private void RPC_PlayerAttack(int damage)
    {
        // Apply damage to the enemy
        enemyCurrentHP -= damage;
        enemyCurrentHP = Mathf.Max(enemyCurrentHP, 0);

        // Update health sliders
        UpdateHealthSliders();
    }

    // RPC method to synchronize ability action across all players
    [PunRPC]
    private void RPC_PlayerAbility(int abilityDamage, int crit)
    {
        if (crit == 1) // Crit hit
        {
            turnFeedText.text = "You attack the enemy with CRIT!";
            enemyCurrentHP -= abilityDamage;
        }
        else
        {
            turnFeedText.text = "You missed! Try again!";
        }

        // Ensure enemy HP doesn't go below 0
        enemyCurrentHP = Mathf.Max(enemyCurrentHP, 0);

        // Update health sliders immediately
        UpdateHealthSliders();
    }

    private void UpdateHealthSliders()
    {
        // Directly update the slider value based on the current health
        playerHealthSlider.value = playerCurrentHP;
        enemyHealthSlider.value = enemyCurrentHP;

        // Optional: Update slider text if needed
        if (playerHealthSlider.GetComponentInChildren<TMP_Text>() != null)
        {
            playerHealthSlider.GetComponentInChildren<TMP_Text>().text = $"{playerCurrentHP}/{playerMaxHP}";
        }
        if (enemyHealthSlider.GetComponentInChildren<TMP_Text>() != null)
        {
            enemyHealthSlider.GetComponentInChildren<TMP_Text>().text = $"{enemyCurrentHP}/{enemyMaxHP}";
        }
    }

    private void EnableButtons(bool enable)
    {
        attackButton.interactable = enable;
        abilityButton.interactable = enable;
    }
}
