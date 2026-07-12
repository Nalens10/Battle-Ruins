using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Combo: Agua + Hielo. Congela al jugador (lo paraliza por completo).
public class FreezeStatusCondition : StatusCondition
{
    public GameObject onApplyVfx;

    protected override void ExecuteOnTurnStart(Stats targetStats)
    {
        // Le vacía la energía del turno (mismo criterio que LossEnergyStatusCondition,
        // pero total en vez de parcial) => no le alcanza para moverse ni usar items.
        targetStats.energy = 0;

        if (this.onApplyVfx != null)
        {
            GameObject effect = Instantiate(this.onApplyVfx, this.targetUnitCreature.transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }
    }

    protected override void ExecuteStatsModifiers(Stats targetStats)
    {

    }
}
