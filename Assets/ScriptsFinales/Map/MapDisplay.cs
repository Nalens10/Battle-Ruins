using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


// Les dejo en verde lo que hace ;)
[System.Serializable]
public struct MapTilePair
{
    public TileType type;
    public Tile visualTile;
}

public class MapDisplay : MonoBehaviour
{
    public MapTilePair[] mapTilePairs;
    public Tilemap targetTilemap;

    public Camera gameCamera;
    public Transform cursor;

    // Marcadores de ruta.
    public GameObject pathMarkerPrfb;
    private List<MapPathMarker> pathMarkers = new List<MapPathMarker>();

    public Transform pathMarkerHolder;

    private PlayerMaster playerMaster;

    void Awake()
    {
        this.playerMaster = FindObjectOfType<PlayerMaster>();
    }

    public void RenderMapData(Map mapdata)
    {
        for (int x = 0; x < mapdata.width; x++)
        {
            for (int y = 0; y < mapdata.height; y++)
            {
                TileType type = mapdata.GetTileType(x, y);

                Tile tile = this.GetTileForType(type);

                this.targetTilemap.SetTile(new Vector3Int(x, -y, 0), tile);
            }
        }
    }

    private Tile GetTileForType(TileType type)
    {
        foreach (var pair in this.mapTilePairs)
        {
            if (pair.type == type)
                return pair.visualTile;
        }

        Debug.LogError("No hay tile para: " + type);
        return null;
    }

    void Update()
    {
        if (InputManager.GetIfMouseHasMoved())
        {
            Vector3 world = this.gameCamera.ScreenToWorldPoint(Input.mousePosition);

            if (GameManager.current.mapManager.IsAGroundTile(world))
            {
                this.cursor.gameObject.SetActive(true);
                this.cursor.position = GameManager.current.mapManager.SnapToTile(world);
            }
            else
            {
                this.cursor.gameObject.SetActive(false);
            }

            if (
                this.playerMaster.hasUnitCreatureSelected &&
                GameManager.current.IsOwnerOnTurn(this.playerMaster.selectedUnitCreature)
            )
            {
                this.HideAllPathMarkers();

                switch (this.playerMaster.status)
                {
                    case PlayerCombatStatus.MOVE:
                        List<Vector3> path = GameManager.current.mapManager.PredictWorldPathFor(
                            this.playerMaster.selectedUnitCreature.transform.position, world
                        );

                        this.DisplayPredictedPath(path);
                        break;
                    case PlayerCombatStatus.ITEMSKILL: 
                        List<Vector3> area = GameManager.current.mapManager.PredictAreaFor(
                            this.playerMaster.selectedUnitCreature.transform.position,
                            this.playerMaster.selectedItemSkill.range
                        );

                        this.DisplayPredictedArea(area);
                        break;
                }

            }
        }

        if (InputManager.GetLeftClickDown())
        {
            this.HideAllPathMarkers();

            Vector3 world = this.gameCamera.ScreenToWorldPoint(Input.mousePosition);
            this.playerMaster.OnSelectionRequested(world);
        }

        if (InputManager.GetRightClickDown() && this.playerMaster.hasUnitCreatureSelected)
        {
            this.HideAllPathMarkers();

            Vector3 world = this.gameCamera.ScreenToWorldPoint(Input.mousePosition);
            this.playerMaster.OnMoveOrItemSkillRequested(world);
        }
    }

    private void DisplayPredictedPath(List<Vector3> path)
    {
        UnitCreature selected = this.playerMaster.selectedUnitCreature;

        int mathMaxSteps = Mathf.Min(selected.CurrentMaxDistance(), path.Count);

        for (int i = 0; i < mathMaxSteps; i++)
        {
            MapPathMarker marker = this.GetMarkerByIndex(i);

            int cost = selected.GetEnergyCostForPathLength(i + 1);
            marker.SetColourUsingPathCost(cost);

            marker.transform.position = path[i];
        }
    }

    private void DisplayPredictedArea(List<Vector3> area)
    {
        for (int i = 0; i < area.Count; i++)
        {
            MapPathMarker marker = this.GetMarkerByIndex(i);

            marker.SetColourUsingPathCost(1);

            marker.transform.position = area[i];
        }
    }

    private void HideAllPathMarkers()
    {
        foreach (var marker in this.pathMarkers)
        {
            marker.Hide();
        }
    }

    public MapPathMarker GetMarkerByIndex(int index)
    {
        if (this.pathMarkers.Count > index)
        {
            MapPathMarker marker = this.pathMarkers[index];

            return marker;
        }

        GameObject go = Instantiate(this.pathMarkerPrfb);
        MapPathMarker newMarker = go.GetComponent<MapPathMarker>();
        this.pathMarkers.Add(newMarker);

        newMarker.transform.SetParent(this.pathMarkerHolder);

        return newMarker;
    }
}