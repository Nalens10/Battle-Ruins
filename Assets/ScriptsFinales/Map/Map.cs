using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Les dejo en verde lo que hace ;)
public enum TileType
{
    // Tipos de tiles posibles
    WALL,
    GROUND,
}

public class Map
{
    // Propiedades de la clase Map
    public int width { get; protected set; }
    public int height { get; protected set; }
    // Matriz que representa los tipos de tiles en el mapa
    private TileType[,] tilesData;

    protected Map(TileType[,] tilesData)
    {
        // Constructor privado para inicializar el mapa con los datos de los tiles
        this.tilesData = tilesData;
        this.width = this.tilesData.GetLength(0);  // GetLength(0) devuelve el tamaño de la primera dimensión (anchura)
        this.height = this.tilesData.GetLength(1);// GetLength(1) devuelve el tamaño de la segunda dimensión (altura)
    }

    public TileType GetTileType(int x, int y)
    {
        // Devuelve el tipo de tile en la posición (x, y) del mapa
        if (x < 0 || y < 0)
        {
            return TileType.WALL;
        }

        if (x >= this.width || y >= this.height)
        {
            return TileType.WALL;
        }
        // Si las coordenadas están fuera de los límites del mapa, se considera que es un muro (WALL)
        return this.tilesData[x, y];
    }

    public static Map CreateWithStringData(string mapData)
    {
        // Crea un mapa a partir de una cadena de texto que representa los datos del mapa
        StringReader reader = new StringReader(mapData);

        // Variables para almacenar el ancho y alto del mapa
        int mapWidth = 0;
        int mapHeight = 0;

        // Lista para almacenar los tipos de tiles de manera plana (1D)
        List<TileType> flatTilesData = new List<TileType>();

        while (true)
        {
            // Leer una línea del mapa
            string line = reader.ReadLine();
            if (line == null)
                break;

            line = line.Trim();
            // Línea vacía. Ignorar.
            if (line.Length == 0)
                continue;
            // Actualizar el ancho del mapa si es mayor que el ancho actual
            mapWidth = line.Length;
            mapHeight++;

            // Convertir cada carácter de la línea en un tipo de tile y agregarlo a la lista
            foreach (var letter in line)
            {
                switch (letter)
                {
                    case '#':
                        flatTilesData.Add(TileType.WALL);
                        break;
                    case '.':
                        flatTilesData.Add(TileType.GROUND);
                        break;
                }
            }
        }

        TileType[,] finalMapTiles = new TileType[mapWidth, mapHeight];

        // Convertir la lista plana de tiles en una matriz 2D
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                finalMapTiles[x, y] = flatTilesData[y * mapWidth + x];
            }
        }

        return new Map(finalMapTiles);
    }
}
