using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Combo: Veneno + Fuego. Explota e inflige daño critico.
// Por defecto se aplica en el proximo inicio de turno del receiver, igual que
// el resto de los status (ver comentario mas abajo si lo queres instantaneo).
public class ExplosionStatusCondition : StatusCondition
{
    public float damagePercent = 0.5f;
    public float criticalMultiplier = 2f;
    public GameObject onApplyVfx;

    protected override void ExecuteOnTurnStart(Stats targetStats)
    {
        int damage = Mathf.RoundToInt(damagePercent * stacks * targetStats.maxhp); 
        int damageTaken = this.targetUnitCreature.DamageWithClamp(damage);

        if (damageTaken != 0)
        {
            MessageManager.current.Send(new ItemSkillHealthModMessage(null, this.targetUnitCreature, -damageTaken, true));
        }

        if (this.onApplyVfx != null)
        {
            GameObject effect = Instantiate(this.onApplyVfx, this.targetUnitCreature.transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        // La explosion es un evento unico: forzamos que se consuma despues de aplicarse.
        this.ConsumeOneTurn();
    }

    protected override void ExecuteStatsModifiers(Stats targetStats)
    {

    }
}
