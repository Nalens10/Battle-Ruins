using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BattleRoyaleZoneManager : MonoBehaviour
{ 
    [Header("References")] public GameObject poisonAreaPrefab; 
    [Header("Gameplay")] public int shrinkPerIATurn = 1; 
    private MapManager mapManager;
    private int[,] ringMap;
    private GameObject[,] poisonObjects;
    private Transform poisonHolder;
    private bool[,] safeTiles;
    private int currentRing;
    private int maxRing;
    private bool initialized = false;
    public void Initialize(MapManager map)
    {
        mapManager = map;

        poisonHolder = new GameObject("Poison Areas").transform;
        poisonHolder.SetParent(transform);

        GenerateSafeTileMap();
        GenerateRingMap();
        SpawnPoisonTiles();
        RefreshPoison();

        initialized = true;
    }
    public void OnIATurn()
    {
        if (!initialized)
        {
            Debug.LogWarning("BattleRoyale todavía no está inicializado.");
            return;
        }

        currentRing += shrinkPerIATurn;

        if (currentRing > maxRing)
            currentRing = maxRing;

        RefreshPoison();
    }
    private void GenerateSafeTileMap()
    {
        safeTiles = new bool[mapManager.Width, mapManager.Height]; 
        foreach (Vector2Int tile in mapManager.safeZoneTiles) 
        { 
            safeTiles[tile.x, tile.y] = true; 
        } 
    } 
    private void GenerateRingMap() 
    { 
        ringMap = new int[mapManager.Width, mapManager.Height]; 
        for (int x = 0; x < mapManager.Width; x++) 
        {
            for (int y = mapManager.Height - 1; y >= 0; y--)
            { 
                ringMap[x, y] = -1;
            } 
        }
        FloodFillFromBorders();
        for (int y = 0; y < mapManager.Height; y++) 
        {
            string line = ""; 
            for (int x = 0; x < mapManager.Width; x++) 
            {
                line += ringMap[x, y].ToString().PadLeft(3);
            } 
            Debug.Log(line); 
        }
    }

    private void FloodFillFromBorders()
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        maxRing = 0;

        // Borde superior e inferior
        for (int x = 0; x < mapManager.Width; x++)
        {
            AddBorderTile(new Vector2Int(x, 0), queue);
            AddBorderTile(new Vector2Int(x, mapManager.Height - 1), queue);
        }

        // Bordes izquierdo y derecho
        for (int y = 0; y < mapManager.Height; y++)
        {
            AddBorderTile(new Vector2Int(0, y), queue);
            AddBorderTile(new Vector2Int(mapManager.Width - 1, y), queue);
        }

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            int nextDistance = ringMap[current.x, current.y] + 1;

            TryVisit(current + Vector2Int.up, nextDistance, queue);
            TryVisit(current + Vector2Int.down, nextDistance, queue);
            TryVisit(current + Vector2Int.left, nextDistance, queue);
            TryVisit(current + Vector2Int.right, nextDistance, queue);
        }
    }

    private void TryVisit(Vector2Int tile, int distance, Queue<Vector2Int> queue) 
    { 
        if (tile.x < 0 || tile.y < 0) 
            return;
        if (tile.x >= mapManager.Width)
            return;
        if (tile.y >= mapManager.Height) 
            return;
        if (ringMap[tile.x, tile.y] != -1) 
            return;
        TileType type = mapManager.map.GetTileType(tile.x, tile.y);

        if (type == TileType.WALL) 
            return; 
        if (safeTiles[tile.x, tile.y])
            return;
        ringMap[tile.x, tile.y] = distance;
        if (distance > maxRing)
            maxRing = distance;

        queue.Enqueue(tile);
    } 

    private void SpawnPoisonTiles()
    {
        poisonObjects = new GameObject[mapManager.Width, mapManager.Height];
        if (poisonAreaPrefab == null)
        {
            Debug.LogError("No hay Poison Area Prefab asignado.");
            return;
        }
        for (int x = 0; x < mapManager.Width; x++)
        {
            for (int y = 0; y < mapManager.Height; y++)
            {
                TileType tile = mapManager.map.GetTileType(x, y);
                if (tile == TileType.WALL) continue;
                if (safeTiles[x, y]) continue;
                Vector3 world = mapManager.LocalToWorld(new Vector2Int(x, y));
                GameObject obj = Instantiate(poisonAreaPrefab, world, Quaternion.identity, poisonHolder);
                obj.SetActive(false);
                poisonObjects[x, y] = obj; 
            }
        }
    }

    private void AddBorderTile(Vector2Int tile, Queue<Vector2Int> queue)
    {
        TileType type = mapManager.map.GetTileType(tile.x, tile.y);

        if (type == TileType.WALL)
            return;

        if (safeTiles[tile.x, tile.y])
            return;

        if (ringMap[tile.x, tile.y] != -1)
            return;

        ringMap[tile.x, tile.y] = 0;

        queue.Enqueue(tile);
    }

    private void RefreshPoison()
    {
        if (mapManager == null)
        {
            Debug.LogError("mapManager es NULL");
            return;
        }

        if (ringMap == null)
        {
            Debug.LogError("ringMap es NULL");
            return;
        }

        if (poisonObjects == null)
        {
            Debug.LogError("poisonObjects es NULL");
            return;
        }

        for (int x = 0; x < mapManager.Width; x++)
        {
            for (int y = 0; y < mapManager.Height; y++)
            {
                GameObject poison = poisonObjects[x, y];

                if (poison == null)
                    continue;

                bool active = ringMap[x, y] <= currentRing;

                poison.SetActive(active);
            }
        }
    }
}