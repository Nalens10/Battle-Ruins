using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEffect : MonoBehaviour, IEffect
{
    [Range(0f, 1f)]
    public float healPercent = 0.2f;

    public void Resolve(UnitCreature emitter, UnitCreature receiver)
    {
        ItemSkill parentSkill = this.GetComponent<ItemSkill>();

        Stats stats = receiver.GetCurrentStats();
        int pointsToHeal = Mathf.RoundToInt(stats.maxhp * this.healPercent);

        int healed = receiver.Heal(pointsToHeal);

        MessageManager.current.Send(new ItemSkillHealthModMessage(parentSkill, receiver, healed, false));
    }
}
