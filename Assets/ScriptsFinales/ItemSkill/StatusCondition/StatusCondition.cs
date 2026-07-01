using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusCondition : MonoBehaviour
{
    public bool isDepleted { get => this.remainingTurns <= 0; }

    public int turnCount = 1;
    protected int remainingTurns;

    protected UnitCreature targetUnitCreature;

    public void Configure(UnitCreature targetCreature)
    {
        this.remainingTurns = this.turnCount;
        this.targetUnitCreature = targetCreature;
    }

    public void ConsumeOneTurn()
    {
        this.remainingTurns--;

        if (this.isDepleted)
        {
            Destroy(this.gameObject);
        }
    }

    public abstract void ApplyOnTurnStart(Stats targetStats);
    public abstract void ApplyStatsModifiers(Stats targetStats);
}
