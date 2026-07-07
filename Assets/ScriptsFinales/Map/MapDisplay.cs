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
    private List<MapActionMarker> pathMarkers = new List<MapActionMarker>();

    public Transform pathMarkerHolder;

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

        this.gameCamera.transform.position = new Vector3(
            this.transform.position.x + (mapdata.width / 2f),
            this.transform.position.y - (mapdata.height / 2f - 1),
            this.gameCamera.transform.position.z
        );

        this.gameCamera.GetComponent<CameraMove>().SetZoom(mapdata.width / 3f);
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
        PlayerMaster playerMaster = GameManager.current.CurrentPlayer;
        if (playerMaster == null)
            return;

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
                playerMaster != null && 
                playerMaster.hasUnitCreatureSelected &&
                GameManager.current.IsOwnerOnTurn(playerMaster.selectedUnitCreature)
            )
            {
                this.HideAllPathMarkers();

                switch (playerMaster.status)
                {
                    case PlayerCombatStatus.MOVE:
                        List<Vector3> path = GameManager.current.mapManager.PredictWorldPathFor(
                            playerMaster.selectedUnitCreature.transform.position, world
                        );

                        this.DisplayPredictedPath(path);
                        break;
                    case PlayerCombatStatus.ITEMSKILL:
                        List<Vector3> reachArea = GameManager.current.mapManager.PredictAreaFor(
                            playerMaster.selectedUnitCreature.transform.position,
                            playerMaster.selectedItemSkill.range
                        );

                        this.DisplayPredictedArea(reachArea);

                        if (GameManager.current.mapManager.IsInsideArea(reachArea, world))
                        {
                            List<Vector3> itemSkillEffectArea = GameManager.current.mapManager.PredictAreaFor(
                                world,
                                playerMaster.selectedItemSkill.area
                            );

                            this.DisplayPredictedArea(itemSkillEffectArea, true);
                        }

                        break;
                }

            }
        }

        if (InputManager.GetLeftClickDown())
        {
            this.HideAllPathMarkers();

            Vector3 world = this.gameCamera.ScreenToWorldPoint(Input.mousePosition);
            if (playerMaster != null)
            {
                playerMaster.OnSelectionRequested(world);
            }
        }

        if (playerMaster != null && InputManager.GetRightClickDown() && playerMaster.hasUnitCreatureSelected)
        {
            this.HideAllPathMarkers();

            Vector3 world = this.gameCamera.ScreenToWorldPoint(Input.mousePosition);
            playerMaster.OnMoveOrItemSkillRequested(world);
        }
    }

    private void DisplayPredictedPath(List<Vector3> path)
    {
        PlayerMaster playerMaster = GameManager.current.CurrentPlayer;

        if (playerMaster == null)
            return;

        UnitCreature selected = playerMaster.selectedUnitCreature;

        int mathMaxSteps = Mathf.Min(selected.CurrentMaxDistance(), path.Count);

        for (int i = 0; i < mathMaxSteps; i++)
        {
            MapActionMarker marker = this.GetNextMarker();

            int cost = selected.GetEnergyCostForPathLength(i + 1);
            marker.ShowForPathUsingCost(cost);

            marker.transform.position = path[i];
        }
    }

    private void DisplayPredictedArea(List<Vector3> area, bool isAction = false)
    {
        for (int i = 0; i < area.Count; i++)
        {
            MapActionMarker marker = this.GetNextMarker();

            if (isAction)
            {
                marker.ShowForItemSkillAction();
            }
            else
            {
                marker.ShowForItemSkillReach();
            }

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

    public MapActionMarker GetNextMarker()
    {
        foreach (var marker in this.pathMarkers)
        {
            if (marker.visible == false)
            {
                return marker;
            }
        }

        GameObject go = Instantiate(this.pathMarkerPrfb);
        MapActionMarker newMarker = go.GetComponent<MapActionMarker>();
        this.pathMarkers.Add(newMarker);

        newMarker.transform.SetParent(this.pathMarkerHolder);

        return newMarker;
    }
}