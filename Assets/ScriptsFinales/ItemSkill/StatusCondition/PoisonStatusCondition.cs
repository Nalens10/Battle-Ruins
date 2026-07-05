using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonStatusCondition : StatusCondition
{
    public float damagePercent = 0.2f;
    public GameObject onApplyVfx;

    public override void ApplyOnTurnStart(Stats targetStats)
    {
        int damage = Mathf.RoundToInt(this.damagePercent * (float)targetStats.maxhp);

        int damageTaken = this.targetUnitCreature.DamageWithClamp(damage);
        if (damageTaken != 0)
        {
            MessageManager.current.Send(new ItemSkillDamageMessage(null, this.targetUnitCreature, damageTaken, false));
        }

        if (this.onApplyVfx != null)
        {
            GameObject effect = Instantiate(this.onApplyVfx, this.targetUnitCreature.transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }
    }

    public override void ApplyStatsModifiers(Stats targetStats)
    {

    }
}