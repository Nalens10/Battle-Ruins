using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LossEnergyStatusCondition : StatusCondition
{
    protected override void ExecuteOnTurnStart(Stats targetStats)
    {
        int impediment = Random.Range(0, 2);
        targetStats.energy -= impediment;
    }

    protected override void ExecuteStatsModifiers(Stats targetStats)
    { }
}