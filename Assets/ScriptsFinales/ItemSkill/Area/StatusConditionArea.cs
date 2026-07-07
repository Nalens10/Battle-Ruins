using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusConditionArea : StatusConditionEffect, IMessageListener
{
    public bool isDepleted
    {
        get => this.remainingTurns <= 0 || this.remainingUses <= 0;
    }

    public int uses = 99;
    protected int remainingUses;

    public int turnCount = 1;
    protected int remainingTurns;

    void Start()
    {
        this.remainingUses = this.uses;
        this.remainingTurns = this.turnCount;
        this.TryToResolveArea();

        if (MessageManager.current == null)
            return;

        if (StatusConditionAreaManager.current == null)
            return;

        MessageManager.current.AddListener(MessageTag.UNITCREATURE_MOVED, this);
        StatusConditionAreaManager.current.AddArea(this);
    }

    public void ConsumeOneTurn()
    {
        this.remainingTurns--;

        if (this.isDepleted)
        {
            MessageManager.current.RemoveListener(MessageTag.UNITCREATURE_MOVED, this);
            Destroy(this.gameObject);
        }
    }

    public void TryToResolveArea()
    {
        UnitCreature posibleUnitCreature = GameManager.current.GetUnitCreatureAtPosition(this.transform.position);
        if (posibleUnitCreature != null)
        {
            this.ResolveArea(posibleUnitCreature);
        }
    }

    protected void ResolveArea(UnitCreature target)
    {
        this.remainingUses--;
        this.Resolve(null, target);
    }

    public void Receive(Message msg)
    {
        UnitCreatureMovedMessage cmm = msg as UnitCreatureMovedMessage;

        bool intersectPosition = GameManager.current.mapManager.AreSameTile(
            cmm.unitCreature.transform.position,
            this.transform.position
        );

        if (intersectPosition)
        {
            this.ResolveArea(cmm.unitCreature);
        }
    }
}
