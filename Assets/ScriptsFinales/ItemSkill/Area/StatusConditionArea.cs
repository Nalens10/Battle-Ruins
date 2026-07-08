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
        MessageManager.current.AddListener(MessageTag.NEXT_TURN, this);
        StatusConditionAreaManager.current.AddArea(this);
    }

    public void ConsumeOneTurn()
    {
        this.remainingTurns--;

        if (this.isDepleted)
        {
            MessageManager.current.RemoveListener(MessageTag.UNITCREATURE_MOVED, this);
            MessageManager.current.RemoveListener(MessageTag.NEXT_TURN, this);

            Destroy(gameObject);
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
        remainingUses--;

        int damage = Mathf.RoundToInt(target.stats.maxhp * 0.05f);

        int damageTaken = target.DamageWithClamp(damage);

        if (damageTaken > 0)
        {
            MessageManager.current.Send(
                new ItemSkillHealthModMessage(
                    null,
                    target,
                    -damageTaken,
                    false));
        }
    }

    public void Receive(Message msg)
    {
        if (msg is UnitCreatureMovedMessage moved)
        {
            bool intersectPosition = GameManager.current.mapManager.AreSameTile(
                moved.unitCreature.transform.position,
                this.transform.position
            );

            if (intersectPosition)
            {
                ResolveArea(moved.unitCreature);
            }
        }

        if (msg is NextTurnMessage)
        {
            TryToResolveArea();
        }
    }
}
