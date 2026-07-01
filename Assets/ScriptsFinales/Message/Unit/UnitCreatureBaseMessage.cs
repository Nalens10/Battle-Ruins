using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitCreatureBaseMessage : Message
{
    public UnitCreature unitCreature { get; protected set; }

    public UnitCreatureBaseMessage(UnitCreature unitCreature)
    {
        this.unitCreature = unitCreature;
    }
}
