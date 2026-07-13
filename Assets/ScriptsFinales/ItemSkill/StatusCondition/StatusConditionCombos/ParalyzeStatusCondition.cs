using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Combo: Agua + Rayo. Paraliza al jugador y le hace daño una ronda mas.
public class ParalyzeStatusCondition : StatusCondition
{
    public float damagePercent = 0.15f;
    public GameObject onApplyVfx;

    protected override void ExecuteOnTurnStart(Stats targetStats)
    {
        // Le vacía la energía del turno (mismo criterio que LossEnergyStatusCondition,
        // pero total en vez de parcial) => no le alcanza para moverse ni usar items.
        targetStats.energy = 0;

        int damage = Mathf.RoundToInt(damagePercent * stacks * targetStats.maxhp);
        int damageTaken = this.targetUnitCreature.DamageWithClamp(damage);

        if (damageTaken != 0)
        {
            MessageManager.current.Send(new ItemSkillHealthModMessage(null, this.targetUnitCreature, -damageTaken, false));
        }

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
