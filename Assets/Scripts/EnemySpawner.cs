using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform player;
    public float spawnInterval = 1f;
    public float enemyRadius = 0.5f;

    private float screenHalfWidth;

    void Start()
    {
        // Calculate half the screen width in world units
        screenHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;

        // Start spawning enemies
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        // Calculate a random angle around the player
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        // Calculate the spawn position just off-screen
        Vector3 spawnPosition = player.position + new Vector3(
            Mathf.Cos(angle) * (screenHalfWidth + enemyRadius),
            Mathf.Sin(angle) * (screenHalfWidth + enemyRadius),
            0f
        );

        // Instantiate the enemy
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        // Set the enemy's max health and scale
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.SetTarget(player);
            
            int maxHealth = Random.Range(8, 13); // Random max health between 8 and 12
            enemyScript.Initialize(maxHealth);
        }
    }
}
