using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

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

    public PlayerMaster playerMasterPrefab;

    public Transform playerMasterHolder;

    public BattleRoyaleZoneManager battleRoyale;

    public ItemSpawnerManager itemSpawner;

    void Awake()
    {
        current = this;
    }

    void Start()
    {

        gameUnitCreatures = new List<UnitCreature>();
        graveyard = new List<UnitCreature>();

        returnBuffer = new List<UnitCreature>();

        mapManager = GetComponent<MapManager>();
        mapManager.Configure();

        itemSpawner.Initialize(mapManager);

        itemSpawner.SpawnItems(6);

        battleRoyale.Initialize(mapManager);

        battleRoyale = FindFirstObjectByType<BattleRoyaleZoneManager>();

        if (battleRoyale == null)
        {
            Debug.LogError("No se encontró BattleRoyaleZoneManager en el GameManager.");
        }

        List<Master> masterList = new List<Master>();

        int spawnIndex = 0;

        foreach (PlayerData player in CharacterSelection.Instance.players)
        {
            if (!player.isPlaying)
                continue;

            PlayerMaster master =
                Instantiate(playerMasterPrefab, playerMasterHolder);

            master.Initialize(player);

            master.SpawnUnitCreatures(
                new List<Vector3>()
                {
            mapManager.playerSpawnPoints[spawnIndex]
                });

            masterList.Add(master);

            spawnIndex++;
        }

        IAMaster ia = GetComponentInChildren<IAMaster>();

        ia.SpawnUnitCreatures(mapManager.iaSpawnPoints);

        masterList.Add(ia);

        masters = masterList.ToArray();

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

        
        if (currentMaster is IAMaster)
        {
            battleRoyale.OnIATurn();
            
            itemSpawner.SpawnItems(4);

            IAMaster ia = GetComponentInChildren<IAMaster>();
            ia.enemyCount= 3;
            ia.SpawnUnitCreatures(mapManager.iaSpawnPoints);
        }
    }

    protected void CheckForGameOver()
    {
        List<PlayerMaster> alivePlayers = new List<PlayerMaster>();

        foreach (Master master in masters)
        {
            if (master is PlayerMaster player && player.HasAliveUnitCreatures())
            {
                alivePlayers.Add(player);
            }
        }

        // Todos los jugadores murieron
        if (alivePlayers.Count == 0)
        {
            Debug.Log("Todos los jugadores fueron eliminados.");
            isGameOver = true;
            return;
        }

        // Sólo queda un jugador humano
        if (alivePlayers.Count == 1)
        {
            isGameOver = true;

            PlayerMaster winner = alivePlayers[0];

            MessageManager.current.Send(new GameOverMessage(winner, null));

            Debug.Log("Ganador: " + winner.masterName);
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

    public void TryToPerformItemSkillAtPoint( UnitCreature emitter,ItemInstance item,Vector3 point)
    {
        Vector3 targetPos = this.mapManager.SnapToTile(point);

        ItemSkill skill = item.itemSkill;

        List<Vector3> reachArea = this.mapManager.PredictAreaFor(
            emitter.transform.position,
            skill.range
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

        if (!isInArea)
        {
            Debug.Log("Can't attack. Target is not in range.");
            return;
        }

        List<Vector3> effectArea = this.mapManager.PredictAreaFor(
            targetPos,
            skill.area
        );

        TryToPerformInventoryItem(emitter, item, effectArea);
    }

    public bool TryToPerformItemSkillInArea(UnitCreature emitter,ItemSkill itemSkill,List<Vector3> area)
    {
        if (this.isGameOver)
        {
            return false;
        }

        if (this.IsOwnerOnTurn(emitter) == false)
        {
            Debug.LogWarning("It's not your turn!");
            return false;
        }

        if (emitter.CanExecuteItemSkill(itemSkill) == false)
        {
            Debug.LogWarning("Can't execute skill. No energy.");
            return false;
        }

        if (itemSkill is UniqueItemSkill)
        {
            if (!emitter.CanUseUniqueSkill())
            {
                Debug.Log("Unique Skill en cooldown.");
                return false;
            }
        }

        if (itemSkill.isSpawner)
        {
            if (itemSkill is UniqueItemSkill)
            {
                emitter.ConsumeUniqueSkill();
            }

            emitter.ConsumeEnergyFor(itemSkill);
            itemSkill.ResolveAsSpawner(emitter, area);

            ItemViewerManager.current.Hide();

            return true;
        }


        if (this.ThereIsTargetInArea(area) == false)
        {
            Debug.Log("There is no target to execute Skill");
            return false;
        }

        emitter.ConsumeEnergyFor(itemSkill);

        if (itemSkill is UniqueItemSkill)
        {
            emitter.ConsumeUniqueSkill();
        }

        foreach (var point in area)
        {
            UnitCreature receiver = this.GetUnitCreatureAtPosition(point);

            if (receiver != null)
            {
                itemSkill.ResolveForReceiver(emitter, receiver);
            }
        }
        ItemViewerManager.current.Hide();
        return true;
    }

    public bool TryToPerformInventoryItem(UnitCreature emitter,ItemInstance item,List<Vector3> area)
    {
        bool ok = TryToPerformItemSkillInArea( emitter, item.itemSkill,area);

        if (ok)
        {
            emitter.UseInventoryItem(item);
        }
        ItemViewerManager.current.Hide();
        return ok;
    }

    public bool IsOwnerOnTurn(UnitCreature unitCreature)
    {
        Master currentMaster = this.masters[this.turnIndex];
        return unitCreature.master == currentMaster;
    }

    public PlayerMaster CurrentPlayer
    {
        get
        {
            if (masters == null)
                return null;

            if (turnIndex < 0 || turnIndex >= masters.Length)
                return null;

            return masters[turnIndex] as PlayerMaster;
        }
    }

    public WorldItem FindItemAtPosition(Vector3 worldPos)
    {
        WorldItem[] items = FindObjectsOfType<WorldItem>();

        foreach (WorldItem item in items)
        {
            if (mapManager.AreSameTile(item.transform.position, worldPos))
                return item;
        }

        return null;
    }
}