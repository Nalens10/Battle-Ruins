using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Les dejo en verde lo que hace ;)

public enum PlayerCombatStatus
{
    MOVE, ITEMSKILL
}

public class PlayerMaster : Master, IMessageListener
{
    public PlayerData playerData;

    public ItemInstance selectedInventoryItem { get; protected set; }
    public void Initialize(PlayerData data)
    {
        playerData = data;
        masterName = data.playerName;
    }

    public PlayerCombatStatus status { get; protected set; }

    public UnitCreature selectedUnitCreature { get; protected set; }
    public bool hasUnitCreatureSelected
    {
        get => this.selectedUnitCreature != null;
    }

    public ItemSkill selectedItemSkill { get; protected set; }

    void Start()
    {
        MessageManager.current.AddListener(MessageTag.ACTION_UNITCREATURE_MOVE, this);
        MessageManager.current.AddListener(MessageTag.ACTION_UNITCREATURE_ITEMSKILL, this);
    }

    public override void BeginTurn()
    {
        this.GoToMoveMode();
        this.BeginTurnToAllUnitCreatures();
    }

    public void OnSelectionRequested(Vector3 worldPos)
    {
        if (status == PlayerCombatStatus.ITEMSKILL)
        {
            // no cambiar de modo
        }
        else
        {
            GoToMoveMode();
        }

        Vector3 targetPos = GameManager.current.mapManager.SnapToTile(worldPos);

        if (this.hasUnitCreatureSelected)
        {
            this.selectedUnitCreature.SetSelectionStatus(false);
        }

        this.selectedUnitCreature = GameManager.current.GetUnitCreatureAtPosition(targetPos);
        if (this.hasUnitCreatureSelected)
        {
            this.selectedUnitCreature.SetSelectionStatus(true);
        }

        MessageManager.current.Send(new UnitCreatureSelectedMessage(this.selectedUnitCreature));
    }

    public void OnMoveOrItemSkillRequested(Vector3 worldPos)
    {
        switch (this.status)
        {
            case PlayerCombatStatus.MOVE:
                GameManager.current.MoveUnitCreatureTo(this.selectedUnitCreature, worldPos);
                break;
            case PlayerCombatStatus.ITEMSKILL:

                if (this.selectedInventoryItem != null)
                {
                    GameManager.current.TryToPerformItemSkillAtPoint(
                        this.selectedUnitCreature,
                        this.selectedInventoryItem,
                        worldPos);
                }
                else
                {
                    GameManager.current.TryToPerformItemSkillAtPoint(
                        this.selectedUnitCreature,
                        this.selectedItemSkill,
                        worldPos);
                }

                this.GoToMoveMode();
                break;
        }
    }

    public void Receive(Message msg)
    {
        if (msg is UnitCreatureActionMoveMessage)
        {
            this.GoToMoveMode();
        }

        if (msg is UnitCreatureActionItemSkillMessage)
        {
            UnitCreatureActionItemSkillMessage casm =
                msg as UnitCreatureActionItemSkillMessage;

            if (casm.inventoryItem != null)
            {
                this.GoToSkillMode(casm.inventoryItem);
            }
            else
            {
                this.GoToSkillMode(casm.itemSkill);
            }
        }

    }

    public void GoToMoveMode()
    {
        this.selectedItemSkill = null;
        this.selectedInventoryItem = null;

        this.status = PlayerCombatStatus.MOVE;
    }

    public void GoToSkillMode(ItemSkill itemSkill)
    {
        this.selectedItemSkill = itemSkill;

        this.status = PlayerCombatStatus.ITEMSKILL;
    }

    public void GoToSkillMode(ItemInstance item)
    {
        this.selectedInventoryItem = item;
        this.selectedItemSkill = item.itemSkill;

        this.status = PlayerCombatStatus.ITEMSKILL;
    }

    public override void SpawnUnitCreatures(List<Vector3> spawnPoints)
    {
        GameObject go = Instantiate(playerData.selectedCharacter.unitCreaturePrfbs);

        UnitCreature unit = go.GetComponent<UnitCreature>();

        unit.uniqueSkill = Instantiate(playerData.selectedCharacter.uniqueSkill);

        unit.transform.position = spawnPoints[0];

        AdoptUnitCreature(unit);
    }

    public void GoToUniqueSkillMode()
    {
        selectedInventoryItem = null;

        PlayerMaster player = selectedUnitCreature.master as PlayerMaster;

        UniqueItemSkill skill = player.playerData.selectedCharacter.uniqueSkill;

        status = PlayerCombatStatus.ITEMSKILL;
    }
}
