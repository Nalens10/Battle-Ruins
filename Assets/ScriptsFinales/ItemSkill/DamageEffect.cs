using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DamageType
{
    PHYSICAL,
    ELEMENTAL,
    MIXED
}

public class DamageEffect : MonoBehaviour, IEffect
{
    public DamageType damageType;
    public ElementalType elementalType;

    public int power = 20;

    public void Resolve(UnitCreature emitter, UnitCreature receiver)
    {
        ItemSkill parentSkill = this.GetComponent<ItemSkill>();

        Stats eStats = emitter.GetCurrentStats();
        Stats rStats = receiver.GetCurrentStats();

        int damage = this.CalculateDamage(eStats, rStats);

        bool isCritical = this.IsCritical(eStats, rStats, parentSkill.currentDistancePenalization);
        if (isCritical)
        {
            damage *= 2;
        }

        MessageManager.current.Send(new ItemSkillDamageMessage(parentSkill, receiver, damage, isCritical));
        receiver.ModifyHealth(-damage);
    }

    protected int CalculateDamage(Stats emitterStats, Stats receiverStats)
    {
        
        float AD = this.CalculateAD(emitterStats, receiverStats);
        float rawDamage = (2f * this.power * AD);
        rawDamage = (rawDamage / 50f) + 2f;

        rawDamage *= this.GetElementalMultiplier(this.elementalType, receiverStats.elementalType);

        return Mathf.RoundToInt(rawDamage);
    }

    protected float CalculateAD(Stats emitterStats, Stats receiverStats)
    {
        if (this.damageType == DamageType.PHYSICAL)
        {
            return emitterStats.attack / receiverStats.defense;
        }

        if (this.damageType == DamageType.ELEMENTAL)
        {
            return emitterStats.elemAttack / receiverStats.elemDefense;
        }

        if (this.damageType == DamageType.MIXED)
        {
            float physical = (emitterStats.attack / receiverStats.defense) / 2f;
            float elemental = (emitterStats.elemAttack / receiverStats.elemDefense) / 2f;

            return physical + elemental;
        }

        return 0;
    }

    protected float GetElementalMultiplier(ElementalType skillType, ElementalType receiverType)
    {
        float multiplier = 1f;

        multiplier *= ElementalWeaknessDB.GetWeaknessMultiplier(skillType, receiverType);

        return multiplier;
    }

    protected bool IsCritical(Stats eStats, Stats rStats, float distancePenalization)
    {
        float critChance = Mathf.Max(eStats.accuracy - rStats.evasion, 0) / (float)eStats.accuracy;
        critChance += distancePenalization;

        float dice = Random.Range(0f, 1f);

        return dice < critChance;
    }
}
