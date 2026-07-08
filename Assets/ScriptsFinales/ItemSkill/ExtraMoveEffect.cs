using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraMoveEffect : MonoBehaviour, IEffect
{
    public void Resolve(UnitCreature emitter, UnitCreature receiver)
    {
        emitter.stats.energy += 1;

        MessageManager.current.Send(
            new UnitCreatureUpdatedMessage(emitter));
    }
}
