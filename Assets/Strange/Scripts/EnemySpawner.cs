using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Drag in the prefab of the enemy you want to spawn here")]
    public GameObject enemyPrefab;

    public List<GameObject> enemies; // enemies in the scene that belong to this spawner

    public int maxNumberofEnemies = 1;
    public int currentNumberOfEnemies = 0;

    public int spawnRadius = 10; // the maximum distance an enemy can spawn from the spawner
    public int returnToSpawnRadius = 15; // while chasing the player, an enemy will forget the player and return to the spawner at this distance


    /* when an enemy spawns a player it sandomly selects a position within spawnRadius
     * then sends a raycast down and spawns the enemy wherever the ray hits.
     * this value sets the maximum length of the ray
     */
    public int maxHeightOffset = 10;

    public bool SpawnEnemiesInAir = false;




    // Start is called before the first frame update
    void Start()
    {
        if(enemyPrefab == null)
        {
            StrangeLogger.LogError("Enemy Spawner is missing enemy prefab");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(currentNumberOfEnemies < maxNumberofEnemies)
        {
            // generate random position within spawnRadius
            Vector2 randomRadius = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = this.transform.position + new Vector3(randomRadius.x,0, randomRadius.y);

            // raycast down
            RaycastHit raycastInfo;
            bool hit;
            if (!SpawnEnemiesInAir)
            {
                hit = Physics.Raycast(spawnPos, Vector3.down, out raycastInfo, maxHeightOffset);
            }
            else
            {
                raycastInfo = new RaycastHit();
                raycastInfo.point = spawnPos;
                hit = true;
            }

            // if raycast hits anything, spawn enemy at that location with random roation
            if (hit)
            {
                GameObject tempEnemy = Instantiate(enemyPrefab, raycastInfo.point + new Vector3(0, enemyPrefab.GetComponent<EnemyComponent>().enemyReference.yOffset, 0) , Quaternion.Euler(0, Random.Range(1,360) ,0));
                tempEnemy.GetComponent<EnemyComponent>().parentSpawner = this;
                enemies.Add(tempEnemy);
                currentNumberOfEnemies++;
            }
        }
    }
}
