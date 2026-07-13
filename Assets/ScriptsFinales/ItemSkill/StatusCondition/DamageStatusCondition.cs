using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageStatusCondition : StatusCondition
{
    public float damagePercent = 0.2f;
    public GameObject onApplyVfx;


    protected override void ExecuteOnTurnStart(Stats targetStats)
    {
        int damage = Mathf.RoundToInt(damagePercent * stacks *targetStats.maxhp);

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