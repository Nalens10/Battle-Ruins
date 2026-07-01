using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ItemSkillMissMessage : Message
{
    public override MessageTag tag => MessageTag.ITEMSKILL_MISS;

    public ItemSkill item { get; protected set; }
    public UnitCreature receiver { get; protected set; }

    public ItemSkillMissMessage(ItemSkill item, UnitCreature receiver)
    {
        this.item = item;
        this.receiver = receiver;
    }

    public override string ToString()
    {
        return $"Skill: {this.item.itemSkillName} MISS";
    }
}
