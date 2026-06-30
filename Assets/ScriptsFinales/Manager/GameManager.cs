using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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
        Master ia = this.GetComponentInChildren<IAMaster>();

        this.masters = new Master[] { player, ia };

        player.SpawnUnitCreatures(this.mapManager.playerSpawnPoints);
        ia.SpawnUnitCreatures(this.mapManager.iaSpawnPoints);

        this.turnIndex = -1;
        this.NextTurn();
    }

    public void EmplaceUnitCreature(UnitCreature unitCreature, Vector3 worldPosition)
    {
        if (this.mapManager.IsAGroundTile(worldPosition) == false)
        {
            throw new System.Exception("Invalid Creature emplacement!");
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
        foreach (var creature in this.gameUnitCreatures)
        {
            float distance = Vector2.Distance(worldPosition, creature.transform.position);

            if (distance < 1f)
            {
                return creature;
            }
        }

        return null;
    }

    public void MoveUnitCreatureTo(UnitCreature unitCreature, Vector3 worldTarget)
    {
        if (this.IsOwnerOnTurn(unitCreature) == false)
        {
            Debug.LogError("Cannot move this creature.");
            return;
        }

        List<Vector3> path = this.mapManager.PredictWorldPathFor(unitCreature.transform.position, worldTarget);
        unitCreature.FollowPath(path.ToArray());
    }

    public void TryToPerformItemSkill(UnitCreature emitter, UnitCreature receiver, ItemSkill itemSkill)
    {
        if (this.IsOwnerOnTurn(emitter) == false)
        {
            Debug.LogError("It's not your turn!");
            return;
        }

        if (emitter.CanExecuteItemSkill(itemSkill) == false)
        {
            Debug.LogError("Can't execute ItemSkill. No energy.");
            return;
        }

        emitter.ConsumeEnergyFor(itemSkill);
        itemSkill.Resolve(emitter, receiver);
    }

    public bool IsOwnerOnTurn(UnitCreature unitCreature)
    {
        Master currentMaster = this.masters[this.turnIndex];
        return unitCreature.master == currentMaster;
    }
}