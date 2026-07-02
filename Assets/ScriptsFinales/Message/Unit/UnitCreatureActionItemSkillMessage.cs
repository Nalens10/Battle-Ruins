using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCreatureActionItemSkillMessage : UnitCreatureBaseMessage
{
    public override MessageTag tag => MessageTag.ACTION_UNITCREATURE_ITEMSKILL;

    public ItemSkill itemSkill { get; protected set; }

    public UnitCreatureActionItemSkillMessage(UnitCreature unitCreature, ItemSkill itemSkill) : base(unitCreature)
    {
        this.itemSkill = itemSkill;
    }
}
