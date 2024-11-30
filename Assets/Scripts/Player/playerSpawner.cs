using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    [Header("Class Settings")]
    [SerializeField] private List<Sprite> classSprites; // List of sprites for each class
    [SerializeField] private string playerPrefabName = "Player"; // Name of your player prefab in Resources

    [Header("Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints; // Array of predefined spawn points

    private const string ClassPropertyKey = "PlayerClass"; // Key for the player's chosen class in custom properties

    private Dictionary<int, GameObject> spawnedPlayers = new Dictionary<int, GameObject>(); // Tracks spawned players

    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            return;
        }

        // Spawn the local player's object with their selected class
        SpawnPlayers();
    }

    private void SpawnPlayers()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue(ClassPropertyKey, out object classIndexObj) && classIndexObj is int classIndex)
            {
                // Only spawn local player's object if it's not already spawned
                if (!spawnedPlayers.ContainsKey(player.ActorNumber))
                {
                    SpawnPlayerObject(player, classIndex);
                }
            }
            else
            {
                Debug.LogWarning($"Player {player.NickName} does not have a class assigned.");
            }
        }
    }

    private void SpawnPlayerObject(Player player, int classIndex)
    {
        // Determine the spawn point
        Transform spawnPoint = GetSpawnPoint(player.ActorNumber);

        if (spawnPoint == null)
        {
            Debug.LogError("No spawn point available. Ensure spawn points are assigned in the Inspector.");
            return;
        }

        // Instantiate the player prefab at the specified spawn point
        GameObject playerObject = PhotonNetwork.Instantiate(playerPrefabName, spawnPoint.position, spawnPoint.rotation);

        // Assign the appropriate sprite based on the class choice
        if (playerObject.TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            if (classIndex >= 0 && classIndex < classSprites.Count)
            {
                spriteRenderer.sprite = classSprites[classIndex];
            }
            else
            {
                Debug.LogError($"Invalid class index {classIndex} for player {player.NickName}.");
            }
        }

        // Track the spawned object
        spawnedPlayers[player.ActorNumber] = playerObject;

        Debug.Log($"Spawned {player.NickName} at {spawnPoint.position} with class index {classIndex}.");
    }

    private Transform GetSpawnPoint(int playerActorNumber)
    {
        // Choose a spawn point based on the player's actor number
        int spawnIndex = (playerActorNumber - 1) % spawnPoints.Length;

        if (spawnIndex < 0 || spawnIndex >= spawnPoints.Length)
        {
            Debug.LogWarning($"Spawn point index {spawnIndex} is out of range.");
            return null;
        }

        return spawnPoints[spawnIndex];
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        // If the new player has chosen a class, spawn their object
        if (newPlayer.CustomProperties.TryGetValue(ClassPropertyKey, out object classIndexObj) && classIndexObj is int classIndex)
        {
            SpawnPlayerObject(newPlayer, classIndex);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        // Clean up the spawned object for the player who left
        if (spawnedPlayers.TryGetValue(otherPlayer.ActorNumber, out GameObject playerObject))
        {
            Destroy(playerObject);
            spawnedPlayers.Remove(otherPlayer.ActorNumber);
        }
    }
}
