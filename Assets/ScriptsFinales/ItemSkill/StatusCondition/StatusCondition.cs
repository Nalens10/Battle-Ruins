using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusConditionFilterType
{
    ARE_IMMUNE,
    ARE_VULNERABLE
}


public abstract class StatusCondition : MonoBehaviour
{
    public bool isDepleted { get => this.remainingTurns <= 0; }

    public string conditionName;

    public GameObject gfx;

    [Header("Immunities")]
    public StatusConditionFilterType filterType = StatusConditionFilterType.ARE_IMMUNE;
    public ElementalType[] typesFilter;

    public int turnCount = 1;
    public int remainingTurns { get; protected set; }

    protected UnitCreature targetUnitCreature;

    void Awake()
    {
        if (this.gfx != null)
        {
            this.gfx.SetActive(false);
        }
    }

    public void Configure(UnitCreature targetUnitCreature)
    {
        this.remainingTurns = this.turnCount;
        this.targetUnitCreature = targetUnitCreature;

        if (this.IsUnitCreatureImmune())
        {
            this.remainingTurns = 0;
        }
        else
        {
            if (this.gfx != null)
            {
                this.gfx.SetActive(true);
            }
        }
    }

    protected bool IsUnitCreatureImmune()
    {
        Stats creatureUnitStats = this.targetUnitCreature.GetCurrentStats();

        if (this.filterType == StatusConditionFilterType.ARE_IMMUNE)
        {
            foreach (var elemType in this.typesFilter)
            {
                if (creatureUnitStats.elementalType == elemType)
                {
                    return true;
                }
            }

            return false;
        }
        else if (this.filterType == StatusConditionFilterType.ARE_VULNERABLE)
        {
            foreach (var elemType in this.typesFilter)
            {
                if (creatureUnitStats.elementalType == elemType)
                {
                    return false;
                }
            }

            return true;
        }

        throw new System.Exception("IsCreatureInmune: Unreachable!");
    }

    public void ConsumeOneTurn()
    {
        this.remainingTurns--;

        if (this.isDepleted)
        {
            Destroy(this.gameObject);
        }
    }

    public void ApplyOnTurnStart(Stats targetStats)
    {
        if (this.isDepleted)
        {
            return;
        }

        this.ExecuteOnTurnStart(targetStats);
    }

    public void ApplyStatsModifiers(Stats targetStats)
    {
        if (this.isDepleted)
        {
            return;
        }

        this.ExecuteStatsModifiers(targetStats);
    }

    protected abstract void ExecuteOnTurnStart(Stats targetStats);
    protected abstract void ExecuteStatsModifiers(Stats targetStats);

    public void Refresh()
    {
        this.remainingTurns = this.turnCount;
    }
}
