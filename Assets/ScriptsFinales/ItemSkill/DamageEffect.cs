using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : MonoBehaviour, IEffect
{
    public int power = 20;

    public void Resolve(UnitCreature emitter, UnitCreature receiver)
    {
        int damage = this.CalculateDamage(emitter.GetCurrentStats(), receiver.GetCurrentStats());

        receiver.ModifyHealth(-damage);
    }

    protected int CalculateDamage(Stats emitterStats, Stats receiverStats)
    {
        float attackDefenseRatio = (float)emitterStats.attack / receiverStats.defense;

        float rawDamage = this.power * attackDefenseRatio;

        return Mathf.RoundToInt((rawDamage / 50f) + 2f);
    }
}
