
using System.Collections.Generic;
using UnityEngine;

// Les dejo en verde lo que hace ;)
public class GameManager : MonoBehaviour
{
    public static GameManager current;

    private Master[] masters;
    private int turnIndex;

    public MapManager mapManager { get; protected set; }

    protected List<UnitCreature> gameUnitCreatures;
    protected List<UnitCreature> graveyard;

    protected bool isGameOver;

    protected List<UnitCreature> returnBuffer;

    void Start()
    {
        current = this;

        this.gameUnitCreatures = new List<UnitCreature>();
        this.graveyard = new List<UnitCreature>();

        this.returnBuffer = new List<UnitCreature>();

        this.mapManager = GetComponent<MapManager>();
        this.mapManager.Configure();

        Master player = this.GetComponentInChildren<PlayerMaster>();
        Master ia = this.GetComponentInChildren<IAMaster>();

        this.masters = new Master[] { player, ia };

        player.SpawnUnitCreatures(this.mapManager.playerSpawnPoints);
        ia.SpawnUnitCreatures(this.mapManager.iaSpawnPoints);

        this.turnIndex = -1;
        this.isGameOver = false;

        Invoke("NextTurn", .5f);
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

    public void OnUnitCreatureDeath(UnitCreature unitCreature)
    {
        this.gameUnitCreatures.Remove(unitCreature);
        unitCreature.gameObject.SetActive(false);

        this.graveyard.Add(unitCreature);

        this.CheckForGameOver();
    }

    public void NextTurn()
    {
        if (this.isGameOver)
        {
            return;
        }

        this.turnIndex = (this.turnIndex + 1) % this.masters.Length;

        Master currentMaster = this.masters[this.turnIndex];

        this.mapManager.dynamicObstacles.Clear();

        foreach (var unitCreature in this.gameUnitCreatures)
        {
            if (unitCreature.master != currentMaster)
            {
                this.mapManager.dynamicObstacles.Add(unitCreature.transform.position);
            }
        }

        MessageManager.current.Send(new NextTurnMessage(currentMaster));
        Debug.Log(currentMaster.masterName);
        currentMaster.BeginTurn();
    }

    protected void CheckForGameOver()
    {
        int unitCreatureCount = this.gameUnitCreatures.Count;
        if (unitCreatureCount == 0)
        {
            Debug.LogWarning("Empate!!  ??");
            this.isGameOver = true;
        }

        Master player = this.masters[0];
        Master ia = this.masters[1];

        if (player.HasAliveUnitCreatures() && ia.HasAliveUnitCreatures() == false)
        {
            this.isGameOver = true;
            MessageManager.current.Send(new GameOverMessage(player, ia));
        }

        if (ia.HasAliveUnitCreatures() && player.HasAliveUnitCreatures() == false)
        {
            this.isGameOver = true;
            MessageManager.current.Send(new GameOverMessage(ia, player));
        }
    }

    public UnitCreature GetUnitCreatureAtPosition(Vector3 worldPosition)
    {
        foreach (var unitCreature in this.gameUnitCreatures)
        {
            float distance = Vector2.Distance(worldPosition, unitCreature.transform.position);

            if (distance < 1f)
            {
                return unitCreature;
            }
        }

        return null;
    }

    public List<UnitCreature> GetEnemyUnitCreaturesInArea(List<Vector3> area, Master currentMaster)
    {
        this.returnBuffer.Clear();

        foreach (var point in area)
        {
            UnitCreature posibleCreature = this.GetUnitCreatureAtPosition(point);
            if (posibleCreature == null)
            {
                continue;
            }

            if (posibleCreature.master == currentMaster)
            {
                continue;
            }

            this.returnBuffer.Add(posibleCreature);
        }

        return this.returnBuffer;
    }

    public bool CanMoveUnitCreatureTo(UnitCreature unitCreature, Vector3 worldTarget)
    {
        Vector3 targetPos = this.mapManager.SnapToTile(worldTarget);

        if (this.IsOwnerOnTurn(unitCreature) == false)
        {
            Debug.LogWarning("Cannot move this creature.");
            return false;
        }

        if (GameManager.current.mapManager.IsAGroundTile(targetPos) == false)
        {
            Debug.LogWarning("Not a ground tile.");
            return false;
        }

        UnitCreature creatureAtLocation = this.GetUnitCreatureAtPosition(targetPos);
        if (creatureAtLocation != null)
        {
            Debug.Log("Tile already occupied.");
            return false;
        }

        return true;
    }

    public void MoveUnitCreatureTo(UnitCreature unitCreature, Vector3 worldTarget)
    {
        if (this.isGameOver)
        {
            return;
        }

        Vector3 targetPos = this.mapManager.SnapToTile(worldTarget);

        if (this.CanMoveUnitCreatureTo(unitCreature, worldTarget) == false)
        {
            return;
        }

        List<Vector3> path = this.mapManager.PredictWorldPathFor(unitCreature.transform.position, targetPos);
        unitCreature.FollowPath(path.ToArray());
    }

    public bool ThereIsTargetInArea(List<Vector3> area)
    {
        foreach (var point in area)
        {
            UnitCreature unitCreature = this.GetUnitCreatureAtPosition(point);
            if (unitCreature != null)
                return true;
        }

        return false;
    }

    public void TryToPerformItemSkillAtPoint(UnitCreature emitter, ItemSkill itemSkill, Vector3 point)
    {
        Vector3 targetPos = this.mapManager.SnapToTile(point);

        List<Vector3> reachArea = this.mapManager.PredictAreaFor(
            emitter.transform.position,
            itemSkill.range
        );

        bool isInArea = false;
        foreach (var pos in reachArea)
        {
            if (pos == targetPos)
            {
                isInArea = true;
                break;
            }
        }

        if (isInArea == false)
        {
            Debug.Log("Can't attack. Target is not in range.");
            return;
        }

        List<Vector3> effectArea = this.mapManager.PredictAreaFor(
            targetPos,
            itemSkill.area
        );

        this.TryToPerformItemSkillInArea(emitter, itemSkill, effectArea);
    }

    public void TryToPerformItemSkillInArea(UnitCreature emitter, ItemSkill itemSkill, List<Vector3> area)
    {
        if (this.isGameOver)
        {
            return;
        }

        if (this.IsOwnerOnTurn(emitter) == false)
        {
            Debug.LogWarning("It's not your turn!");
            return;
        }

        if (emitter.CanExecuteItemSkill(itemSkill) == false)
        {
            Debug.LogWarning("Can't execute skill. No energy.");
            return;
        }

        if (this.ThereIsTargetInArea(area) == false)
        {
            Debug.Log("There is no target to execute Skill");
            return;
        }

        emitter.ConsumeEnergyFor(itemSkill);

        foreach (var point in area)
        {
            UnitCreature receiver = this.GetUnitCreatureAtPosition(point);
            if (receiver != null)
            {
                itemSkill.Resolve(emitter, receiver);
            }
        }
    }

    public bool IsOwnerOnTurn(UnitCreature unitCreature)
    {
        Master currentMaster = this.masters[this.turnIndex];
        return unitCreature.master == currentMaster;
    }

}