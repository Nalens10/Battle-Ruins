using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LossEnergyStatusCondition : StatusCondition
{
    public override void ApplyOnTurnStart(Stats targetStats)
    {
        int impediment = Random.Range(0, 2);
        targetStats.energy -= impediment;
    }

    public override void ApplyStatsModifiers(Stats targetStats)
    { }
}