using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public List<Transform> zombieSpawnPoints;

    public GameObject zombiePrefab;

    public int roundsBetweenSpawns = 3;

    public void SpawnZombie()
    {
        int index =
            Random.Range(0, zombieSpawnPoints.Count);

        Instantiate(
            zombiePrefab,
            zombieSpawnPoints[index].position,
            Quaternion.identity);
    }

    public void SpawnWave(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            SpawnZombie();
        }
    }
}
