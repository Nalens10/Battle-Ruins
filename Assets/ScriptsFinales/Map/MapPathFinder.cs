using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Les dejo en verde lo que hace ;)
public class MapPathFinder
{
    // Matriz que representa la distancia desde el punto final a cada celda del mapa
    private int[,] distanceMap;
    private List<Vector2Int> path;

    private int mapWidth;
    private int mapHeight;

    public MapPathFinder()
    {
       path = new List<Vector2Int>();
    }

    public void ConfigureForMap(Map map)
    {
        // Configuramos el PathFinder para trabajar con un mapa específico
        mapWidth = map.width;
        mapHeight = map.height;

        distanceMap = new int[map.width, map.height];

        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                // Obtenemos el Tile en esta posición
                var tile = map.GetTileType(x, y);

                // Asignamos el valor de "onda"
                if (tile == TileType.WALL)
                    distanceMap[x, y] = -1;
                else
                    distanceMap[x, y] = 0;
            }
        }
    }

    private void ClearPathData()
    {
        // Clear map
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (distanceMap[x, y] != -1)
                    distanceMap[x, y] = 0;
            }
        }

        path.Clear();
    }   

    private void Visit(Queue<Vector2Int> visitedCells, int distance, int x, int y)
    {
        // Si la celda está fuera del mapa, no hacemos nada
        if (x < 0 || x >= mapWidth)
            return;
        
        if (y < 0 || y >= mapHeight)
            return;

        // Si la celda ya ha sido visitada o es un muro, no hacemos nada
        if (distanceMap[x, y] == 0)
        {
            distanceMap[x, y] = distance;
            visitedCells.Enqueue(new Vector2Int(x, y));
        }
    }

    private bool IsOutsideOfTheMap(int x, int y)
    {
        // Verifica si las coordenadas (x, y) están fuera de los límites del mapa
        if (x < 0 || x >= mapWidth)
            return true;

        if (y < 0 || y >= mapHeight)
            return true;

        return false;
    }

    public List<Vector2Int> GetPath(int startX, int startY, int endX, int endY)
    {
        // Devuelve la lista de coordenadas que representan el camino desde (startX, startY) hasta (endX, endY)
        if (IsOutsideOfTheMap(startX, startY))
            return path;

        if (IsOutsideOfTheMap(endX, endY))
            return path;

        ClearPathData();
        ComputeDistanceMap(startX, startY, endX, endY);
        TracePath(new Vector2Int(startX, startY));

        return  path;
    }   

    private void ComputeDistanceMap(int startX, int startY, int endX, int endY)
    {
        // Si la celda de inicio o la celda final ya han sido visitadas o son muros, no hacemos nada
        if (distanceMap[endX, endY] != 0 || distanceMap[startX, startY] != 0)
        {
            // Sin solución
            return;
        }

        int distance = 1;

        // Creamos una cola para almacenar las celdas visitadas y comenzamos desde la celda final
        Queue<Vector2Int> visitedCells = new Queue<Vector2Int>();
        distanceMap[endX, endY] = distance;
        visitedCells.Enqueue(new Vector2Int(endX, endY));

        // Mientras haya celdas visitadas y no se haya alcanzado un límite de 1000 celdas, seguimos propagando la onda
        while (visitedCells.Count != 0 && visitedCells.Count < 1000)
        {
            distance += 1;

            var cell = visitedCells.Dequeue();
            Visit(visitedCells, distance, cell.x, cell.y + 1); // UP
            Visit(visitedCells, distance, cell.x, cell.y - 1); // Down
            Visit(visitedCells, distance, cell.x + 1, cell.y); // Right
            Visit(visitedCells, distance, cell.x - 1, cell.y); // Left
        }
    }

    private void TracePath(Vector2Int localStart)
    {
        // Comenzamos desde la celda de inicio y seguimos el camino de menor distancia hasta llegar a la celda final
        Vector2Int currentLocalTile = localStart;

        bool first = true;

        bool working = true;
        while (working)
        {
            var (x, y) = ((int)currentLocalTile.x, (int)currentLocalTile.y);
            var d = distanceMap[x, y];

            if (d == -1)
            {
                // Invalid
                return;
            }

            if (!first)
            {
                path.Add(currentLocalTile);
            }
            else
            {
                first = false;
            }

            path.Add(currentLocalTile);

            working = false;
            if (distanceMap[x, y + 1] < d && distanceMap[x, y + 1] != -1)
            {
                currentLocalTile.Set(x, y + 1);
                working = true;
                continue;
            }
            if (distanceMap[x, y - 1] < d && distanceMap[x, y - 1] != -1)
            {
                currentLocalTile.Set(x, y - 1);
                working = true;
                continue;
            }
            if (distanceMap[x + 1, y] < d && distanceMap[x + 1, y] != -1)
            {
                currentLocalTile.Set(x + 1, y);
                working = true;
                continue;
            }
            if (distanceMap[x - 1, y] < d && distanceMap[x - 1, y] != -1)
            {
                currentLocalTile.Set(x - 1, y);
                working = true;
                continue;
            }
        }
    }
}
