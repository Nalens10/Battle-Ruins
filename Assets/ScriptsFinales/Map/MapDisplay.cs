using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


// Les dejo en verde lo que hace ;)
[System.Serializable] public struct MapTilePair
{
    // Estructura que asocia un tipo de tile con su representación visual
    public TileType type;
    public Tile visualTile;
}


public class MapDisplay : MonoBehaviour
{
    // Propiedades de la clase MapDisplay
    public MapTilePair[] mapTilePairs;
    public Tilemap targetTilemap;

    // Variables para la cámara del juego y el cursor
    public Camera gameCamera;
    public Transform cursor;

    private Map map;
    private MapPathFinder pathFinder;

    private UnitCreature selectedUnitCreature;
    private List<UnitCreature> unitCeatures = new List<UnitCreature>();

    // Marcadores de ruta
    public GameObject pathMarkerPrfb;
    private List<MapPathMarker> pathMarkers;

    public Transform pathMarkerHolder;

    void Awake()
    {
        pathMarkers = new List<MapPathMarker>();
    }

    public void RenderMapData(Map mapdata)
    {
        // Renderiza los datos del mapa en el Tilemap
        map = mapdata;

        // Configura el buscador de rutas para el mapa
        pathFinder = new MapPathFinder();
        pathFinder.ConfigureForMap(map);

        // Renderiza cada tile del mapa en el Tilemap
        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                TileType type = map.GetTileType(x, y);

                Tile tile = GetTileForType(type);

                targetTilemap.SetTile(new Vector3Int(x, -y, 0), tile);
            }
        }
    }

    private Tile GetTileForType(TileType type)
    {
        // Busca el tile visual correspondiente al tipo de tile dado
        foreach (var pair in mapTilePairs)
        {
            if (pair.type == type)
                return pair.visualTile;
        }

        Debug.LogError("No hay tile para: " + type);
        return null;
    }

    
    public void EmplaceUnitCreature(UnitCreature unitCreature, Vector2Int localPosition)
    {
        Vector3 worldPosition = LocalToWorld(localPosition);

        unitCreature.localPosition = localPosition;
        unitCreature.transform.position = worldPosition;

        unitCeatures.Add(unitCreature);
    }

    void Update()
    {
        // Actualiza la posición del cursor y maneja la selección de criaturas y el movimiento
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            Vector3 world = this.gameCamera.ScreenToWorldPoint(Input.mousePosition);
            var local = this.WorldToLocal(world);

            if (this.map.GetTileType(local.x, local.y) == TileType.GROUND)
            {
                this.cursor.gameObject.SetActive(true);
                this.cursor.position = this.LocalToWorld(local);
            }
            else
            {
                this.cursor.gameObject.SetActive(false);
            }

            // Si hay una criatura seleccionada, calcula y muestra la ruta hacia la posición del cursor
            if (this.selectedUnitCreature != null)
            {
                Vector2Int start = this.selectedUnitCreature.localPosition;
                List<Vector2Int> path = this.pathFinder.GetPath(start.x, start.y, local.x, local.y);

                this.HideAllPathMarkers();

                this.DisplayPredictedPath(path);
            }
        }

        // Maneja la selección de criaturas con el botón izquierdo del ratón
        if (Input.GetButtonDown("Fire1"))
        {
            this.HideAllPathMarkers();

            Vector3 world = this.gameCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int local = this.WorldToLocal(world);

            if (this.selectedUnitCreature != null)
            {
                this.selectedUnitCreature.SetSelectionStatus(false);
            }

            this.selectedUnitCreature = this.GetUnitCreatureAtPosition(local);
            if (this.selectedUnitCreature != null)
            {
                this.selectedUnitCreature.SetSelectionStatus(true);
            }
        }

        // Maneja el movimiento de la criatura seleccionada con el botón derecho del ratón
        if (Input.GetButtonDown("Fire2") && this.selectedUnitCreature != null)
        {
            Vector3 world = this.gameCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int localTarget = this.WorldToLocal(world);

            Vector2Int localStart = this.selectedUnitCreature.localPosition;
            List<Vector2Int> path = this.pathFinder.GetPath(localStart.x, localStart.y, localTarget.x, localTarget.y);

            this.HideAllPathMarkers();

            if (path.Count != 0)
            {
                this.SetSelectedUnitCreaturePath(path);
            }

        }
    }

    private void SetSelectedUnitCreaturePath(List<Vector2Int> path)
    {
        // Establece la ruta para la criatura seleccionada y la mueve a la posición final de la ruta
        int maxSteps = Mathf.Min(this.selectedUnitCreature.CurrentMaxDistance(), path.Count);
        if (maxSteps == 0) return;
        this.selectedUnitCreature.localPosition = path[maxSteps - 1];

        Vector3[] worldPath = new Vector3[maxSteps];
        for (int i = 0; i < maxSteps; i++)
        {
            worldPath[i] = this.LocalToWorld(path[i]);
        }

        this.selectedUnitCreature.FollowPath(worldPath);
    }

    private void HideAllPathMarkers()
    {
        // Oculta todos los marcadores de ruta
        foreach (var marker in this.pathMarkers)
        {
            marker.Hide();
        }
    }

    private void DisplayPredictedPath(List<Vector2Int> path)
    {
        int mathMaxSteps = Mathf.Min(this.selectedUnitCreature.CurrentMaxDistance(), path.Count);

        for (int i = 0; i < mathMaxSteps; i++)
        {
            Vector3 worldPoint = this.LocalToWorld(path[i]);
            MapPathMarker marker = this.GetMarkerByIndex(i);

            int cost = this.selectedUnitCreature.GetEnergyCostForPathLength(i + 1);
            marker.SetColourUsingPathCost(cost);

            marker.transform.position = worldPoint;
        }
    }


    public MapPathMarker GetMarkerByIndex(int index)
    {
        // Obtiene un marcador de ruta por su índice, si no existe, lo crea
        if (this.pathMarkers.Count > index)
        {
            MapPathMarker marker = this.pathMarkers[index];
            return marker;
        }

        // Si no hay suficientes marcadores, crea uno nuevo
        GameObject go = Instantiate(this.pathMarkerPrfb);
        MapPathMarker newMarker = go.GetComponent<MapPathMarker>();
        this.pathMarkers.Add(newMarker);

        newMarker.transform.SetParent(this.pathMarkerHolder);

        return newMarker;
    }

    private UnitCreature GetUnitCreatureAtPosition(Vector2Int localPosition)
    {
        // Busca una criatura en la lista de criaturas que esté en la posición local especificada 
        foreach (var creature in this.unitCeatures)
        {
            Vector2Int pos = creature.localPosition;
            if (pos.x == localPosition.x && pos.y == localPosition.y)
            {
                return creature;
            }
        }

        return null;
    }

    private Vector2Int WorldToLocal(Vector3 world)
    {
        // Convierte las coordenadas del mundo a coordenadas locales del mapa
        Vector3 local = world - this.transform.position;

        int mapX = Mathf.FloorToInt(local.x);
        int mapY = Mathf.FloorToInt(local.y);

        return new Vector2Int(mapX, -mapY);
    }

    private Vector3 LocalToWorld(Vector2Int local)
    {
        // Convierte las coordenadas locales del mapa a coordenadas del mundo
        Vector3 localF = new Vector3(local.x, -local.y, 0);

        return this.transform.position + localF + (Vector3.one * 0.5f);
    }

    internal void RenderMapData(object map)
    {
        throw new NotImplementedException();
    }
}