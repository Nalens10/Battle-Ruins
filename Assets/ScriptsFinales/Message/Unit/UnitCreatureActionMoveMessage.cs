using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCreatureActionMoveMessage : UnitCreatureBaseMessage
{
    public override MessageTag tag => MessageTag.ACTION_UNITCREATURE_MOVE;

    public UnitCreatureActionMoveMessage(UnitCreature unit) : base(unit)
    {
    }
}