using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCreatureActionItemSkillMessage : UnitCreatureBaseMessage
{
    public override MessageTag tag => MessageTag.ACTION_UNITCREATURE_ITEMSKILL;

    public ItemSkill itemSkill { get; protected set; }

    public ItemInstance inventoryItem { get; protected set; }

    // Habilidad normal
    public UnitCreatureActionItemSkillMessage(
        UnitCreature unitCreature,
        ItemSkill itemSkill) : base(unitCreature)
    {
        this.itemSkill = itemSkill;
        this.inventoryItem = null;
    }

    // Item del inventario
    public UnitCreatureActionItemSkillMessage(
        UnitCreature unitCreature,
        ItemInstance item) : base(unitCreature)
    {
        this.inventoryItem = item;
        this.itemSkill = item.itemSkill;
    }
}
