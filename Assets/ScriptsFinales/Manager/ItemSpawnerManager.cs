using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnerManager : MonoBehaviour
{
    public GameObject worldItemPrefab;

    public List<ItemSkill> possibleItems;

    public int maxItems = 30;

    MapManager map;

    private readonly List<WorldItem> spawnedItems = new List<WorldItem>();

    public void Initialize(MapManager mapManager)
    {
        map = mapManager;
    }

    public void SpawnItems(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            SpawnRandomItem();
        }
    }

    public void SpawnRandomItem()
    {
        if (spawnedItems.Count >= maxItems)
            return;

        if (possibleItems.Count == 0)
        {
            Debug.LogWarning("No hay ItemSkill cargados.");
            return;
        }

        ItemSkill randomSkill =
            possibleItems[Random.Range(0, possibleItems.Count)];

        Vector3 spawnPosition = GetRandomSpawnPosition();

        if (spawnPosition == Vector3.negativeInfinity)
        {
            Debug.LogWarning("No se encontró una posición válida.");
            return;
        }

        GameObject go = Instantiate(worldItemPrefab, spawnPosition, Quaternion.identity);

        WorldItem worldItem = go.GetComponent<WorldItem>();

        worldItem.SetItem(randomSkill);

        spawnedItems.Add(worldItem);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        int attempts = 200;

        while (attempts > 0)
        {
            attempts--;

            Vector3 randomPos = map.GetRandomGroundTile();

            if (GameManager.current.GetUnitCreatureAtPosition(randomPos) != null)
                continue;

            if (IsItemAlreadyHere(randomPos))
                continue;

            return randomPos;
        }

        return Vector3.negativeInfinity;
    }

    private bool IsItemAlreadyHere(Vector3 pos)
    {
        foreach (WorldItem item in spawnedItems)
        {
            if (item == null)
                continue;

            if (Vector3.Distance(item.transform.position, pos) < 0.1f)
                return true;
        }

        return false;
    }

    public void RemoveItem(WorldItem item)
    {
        spawnedItems.Remove(item);
    }

}
