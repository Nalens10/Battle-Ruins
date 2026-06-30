using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;

// Les dejo en verde lo que hace ;)
public class GameManager : MonoBehaviour
{
    public static GameManager current;

    private Master[] masters;
    private int turnIndex;

    public MapManager mapManager { get; protected set; }

    protected List<UnitCreature> gameUnitCreatures;

    void Start()
    {
        current = this;

        this.gameUnitCreatures = new List<UnitCreature>();

        this.mapManager = GetComponent<MapManager>();
        this.mapManager.Configure();

        Master player = this.GetComponentInChildren<PlayerMaster>();
        Master ai = this.GetComponentInChildren<IAMaster>();

        this.masters = new Master[] { player, ai };

        player.SpawnCreatures(this.mapManager.playerSpawnPoints);
        ai.SpawnCreatures(this.mapManager.iaSpawnPoints);

        this.turnIndex = -1;
        this.NextTurn();
    }

    public void EmplaceCreature(UnitCreature unitCreature, Vector3 worldPosition)
    {
        if (this.mapManager.IsAGroundTile(worldPosition) == false)
        {
            throw new System.Exception("Invalid UnitCreature emplacement!");
        }

        unitCreature.transform.position = this.mapManager.SnapToTile(worldPosition);
        this.gameUnitCreatures.Add(unitCreature);
    }

    public void NextTurn()
    {
        this.turnIndex = (this.turnIndex + 1) % this.masters.Length;

        Master currentMaster = this.masters[this.turnIndex];
        TurnUI.current.SetCurrentTurnLabel(currentMaster.masterName);

        currentMaster.BeginTurn();
    }

    public UnitCreature GetUnitCreatureAtPosition(Vector3 worldPosition)
    {
        foreach (var unit in this.gameUnitCreatures)
        {
            float distance = Vector2.Distance(worldPosition, unit.transform.position);

            if (distance < 1f)
            {
                return unit;
            }
        }

        return null;
    }

    public void MoveUnitCreatureTo(UnitCreature unitCreature, Vector3 worldTarget)
    {
        if (this.IsOwnerOnTurn(unitCreature) == false)
        {
            Debug.LogError("Cannot move this UnitCreature.");
            return;
        }

        List<Vector3> path = this.mapManager.PredictWorldPathFor(unitCreature.transform.position, worldTarget);
        unitCreature.FollowPath(path.ToArray());
    }

    public bool IsOwnerOnTurn(UnitCreature creature)
    {
        Master currentMaster = this.masters[this.turnIndex];
        return creature.master == currentMaster;
    }
}