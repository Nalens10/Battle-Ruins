using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCreatureUpdatedMessage : UnitCreatureBaseMessage
{
    public override MessageTag tag => MessageTag.UNITCREATURE_UPDATED;

    public UnitCreatureUpdatedMessage(UnitCreature unitCreature) : base(unitCreature)
    {
    }
}
