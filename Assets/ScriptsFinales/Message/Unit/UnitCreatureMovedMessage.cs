using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCreatureMovedMessage : UnitCreatureBaseMessage
{
    public override MessageTag tag => MessageTag.UNITCREATURE_MOVED;

    public UnitCreatureMovedMessage(UnitCreature unitCreature) : base(unitCreature)
    {

    }
}