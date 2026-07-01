using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

// Les dejo en verde lo que hace ;)

public enum PlayerCombatStatus
{
    MOVE, ITEMSKILL
}

public class PlayerMaster : Master, IMessageListener
{
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
        this.GoToMoveMode();

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
        Vector3 targetPos = GameManager.current.mapManager.SnapToTile(worldPos);

        switch (this.status)
        {
            case PlayerCombatStatus.MOVE:
                GameManager.current.MoveUnitCreatureTo(this.selectedUnitCreature, targetPos);
                break;
            case PlayerCombatStatus.ITEMSKILL:
                List<Vector3> area = GameManager.current.mapManager.PredictAreaFor(
                    this.selectedUnitCreature.transform.position,
                    this.selectedItemSkill.range
                );

                bool isInArea = false;
                foreach (var point in area)
                {
                    if (point == targetPos)
                    {
                        isInArea = true;
                        break;
                    }
                }

                if (isInArea == false)
                {
                    Debug.LogError("Can't attack. Target is not in range.");
                    return;
                }

                UnitCreature posibleTarget = GameManager.current.GetUnitCreatureAtPosition(targetPos);

                if (posibleTarget == null)
                {
                    Debug.LogError("Can't attack. There is no target.");
                    return;
                }

                GameManager.current.TryToPerformItemSkill(this.selectedUnitCreature, posibleTarget, this.selectedItemSkill);
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
            UnitCreatureActionItemSkillMessage casm = msg as UnitCreatureActionItemSkillMessage;
            this.GoToItemSkillMode(casm.itemSkill);
        }
    }


    public void GoToMoveMode()
    {
        this.selectedItemSkill = null;
        this.status = PlayerCombatStatus.MOVE;
    }

    public void GoToItemSkillMode(ItemSkill skill)
    {
        this.selectedItemSkill = skill;

        this.status = PlayerCombatStatus.ITEMSKILL;
    }
}
