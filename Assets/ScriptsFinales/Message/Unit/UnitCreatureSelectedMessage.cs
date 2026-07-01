using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCreatureSelectedMessage : UnitCreatureBaseMessage
{
    public override MessageTag tag => MessageTag.UNITCREATURE_SELECTED;

    public UnitCreatureSelectedMessage(UnitCreature unit) : base(unit)
    {
    }
}
