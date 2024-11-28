using System.Collections;
using UnityEngine;
using TMPro;

public class ZombieManager : MonoBehaviour
{
    public GameObject zombiePrefab;            // Reference to the first zombie prefab
    public GameObject zombiePrefab2;           // Reference to the second zombie prefab
    public Transform[] spawnPoints;            // List of spawn points
    public int initialZombiesPerWave = 10;     // Starting number of zombies per wave
    public TextMeshProUGUI waveText;           // UI text to display wave number

    private int currentWave = 1;               // Track the current wave
    private int zombiesAlive = 0;              // Track the number of zombies alive
    private int zombiesPerWave;                // Dynamic number of zombies per wave

    private void Start()
    {
        zombiesPerWave = initialZombiesPerWave;  // Initialize zombies per wave
        UpdateWaveText();
        SpawnWave();  // Start the first wave
    }

    private void Update()
    {
        // Check if all zombies are killed and spawn the next wave
        if (zombiesAlive == 0)
        {
            currentWave++;
            zombiesPerWave++;  // Increase the number of zombies each wave
            UpdateWaveText();
            SpawnWave();
        }
    }

    // This method will be called by ZombieAI when a zombie dies
    public void OnZombieDeath()
    {
        zombiesAlive--;  // Decrease the count of zombies alive
        Debug.Log("Zombie died. Zombies left: " + zombiesAlive);
    }

    // Spawn a wave of zombies
    private void SpawnWave()
    {
        for (int i = 0; i < zombiesPerWave; i++)
        {
            SpawnZombie();
        }
    }

    // Spawn a single zombie at a random spawn point
    private void SpawnZombie()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Randomly choose between the two zombie prefabs
        GameObject zombieToSpawn = Random.value < 0.5f ? zombiePrefab : zombiePrefab2;

        Instantiate(zombieToSpawn, spawnPoint.position, spawnPoint.rotation);
        zombiesAlive++;  // Increase the count of zombies alive
        Debug.Log("Zombie spawned. Zombies alive: " + zombiesAlive);
    }

    // Update the UI with the current wave number
    private void UpdateWaveText()
    {
        if (waveText != null)
        {
            waveText.text = "Wave: " + currentWave;
        }
    }
}