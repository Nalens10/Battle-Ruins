using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ItemSkillDamageMessage : Message
{
    public override MessageTag tag => MessageTag.ITEMSKILL_DAMAGE;

    public ItemSkill itemSkill { get; protected set; }
    public UnitCreature receiver { get; protected set; }

    public int damage { get; protected set; }
    public bool critical { get; protected set; }

    public ItemSkillDamageMessage(ItemSkill itemSkill, UnitCreature receiver, int damage, bool crit)
    {
        this.itemSkill = this.itemSkill;
        this.receiver = receiver;
        this.damage = damage;
        this.critical = crit;
    }

    public override string ToString()
    {
        string critText = this.critical ? "CRIT" : "";
        return $"{this.itemSkill.itemSkillName} to {this.receiver.name} with {this.damage} {critText}";
    }
}
