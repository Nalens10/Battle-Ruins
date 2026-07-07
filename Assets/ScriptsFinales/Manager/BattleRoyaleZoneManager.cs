using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BattleRoyaleZoneManager : MonoBehaviour
{
    [Header("References")] public GameObject poisonAreaPrefab;

    [Header("Gameplay")] public int shrinkPerIATurn = 1;

    

    private MapManager mapManager;
    private Transform poisonHolder;

    private int[,] tileRing;
    private GameObject[,] poisonTiles;

    //private int currentRing = -1;
    private int maxRing;
    private int currentRing;

    public void Initialize(MapManager manager)
    {
        mapManager = manager;

        if (mapManager == null)
        {
            Debug.LogError("MapManager es null.");
            return;
        }

        if (poisonAreaPrefab == null)
        {
            Debug.LogError("No asignaste el Poison Prefab.");
            return;
        }

        poisonHolder = new GameObject("Poison Areas").transform;
        poisonHolder.SetParent(transform);

        GenerateRingMap();
        SpawnPoisonTiles();
        RefreshPoison();
    }

    private int GetRingForTile(int x, int y)
    {
        int left = x;

        int right = mapManager.Width - 1 - x;

        int top = y;

        int bottom = mapManager.Height - 1 - y;

        return Mathf.Min(left, right, top, bottom);
    }

    private void GenerateRingMap()
    {
        tileRing = new int[mapManager.Width, mapManager.Height];

        maxRing = 0;

        for (int x = 0; x < mapManager.Width; x++)
        {
            for (int y = 0; y < mapManager.Height; y++)
            {
                int ring = GetRingForTile(x, y);

                tileRing[x, y] = ring;

                if (ring > maxRing)
                    maxRing = ring;
            }
        }

        Debug.Log("Último anillo: " + maxRing);
    }

    private void SpawnPoisonTiles()
    {
        poisonTiles = new GameObject[mapManager.Width, mapManager.Height];

        for (int x = 0; x < mapManager.Width; x++)
        {
            for (int y = 0; y < mapManager.Height; y++)
            {
                TileType tile = mapManager.map.GetTileType(x, y);

                if (tile == TileType.WALL)
                    continue;

                Vector2Int localTile = new Vector2Int(x, y);

                if (mapManager.safeZoneTiles.Contains(localTile))
                    continue;

                Vector3 worldPosition =
                    mapManager.LocalToWorld(new Vector2Int(x, y));

                GameObject poison =
                    Instantiate(
                        poisonAreaPrefab,
                        worldPosition,
                        Quaternion.identity,
                        poisonHolder
                    );

                poison.name = $"Poison ({x},{y})";

                poison.SetActive(false);

                poisonTiles[x, y] = poison;
            }
        }
    }

    private void RefreshPoison()
    {
        for (int x = 0; x < mapManager.Width; x++)
        {
            for (int y = 0; y < mapManager.Height; y++)
            {
                GameObject poison = poisonTiles[x, y];

                if (poison == null)
                    continue;

                poison.SetActive(tileRing[x, y] <= currentRing);
            }
        }
    }

    public void OnIATurn()
    {
        currentRing += shrinkPerIATurn;

        if (currentRing > maxRing)
            currentRing = maxRing;

        RefreshPoison();
    }

}