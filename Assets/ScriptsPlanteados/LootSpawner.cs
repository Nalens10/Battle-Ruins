using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    public List<Transform> spawnPoints;

    public List<GameObject> availableLoot;

    public void GenerateInitialLoot()
    {
        foreach (Transform point in spawnPoints)
        {
            SpawnLoot(point);
        }
    }

    public void SpawnLoot(Transform point)
    {
        int randomLoot =
            Random.Range(0, availableLoot.Count);

        Instantiate(
            availableLoot[randomLoot],
            point.position,
            Quaternion.identity);
    }
}
