using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSkillHealthModMessage : Message
{
    public override MessageTag tag => MessageTag.ITEMSKILL_HEALTH_MOD;

    public ItemSkill itemSkill { get; protected set; }
    public UnitCreature receiver { get; protected set; }

    public int healthModAmount { get; protected set; }
    public bool critical { get; protected set; }

    public ItemSkillHealthModMessage(ItemSkill itemSkill, UnitCreature receiver, int healthModAmount, bool crit)
    {
        this.itemSkill = this.itemSkill;
        this.receiver = receiver;
        this.healthModAmount = healthModAmount;
        this.critical = crit;
    }

    public override string ToString()
    {
        string critText = this.critical ? "CRIT" : "";
        return $"{this.itemSkill.itemSkillName} to {this.receiver.name} with {this.healthModAmount} {critText}";
    }
}
