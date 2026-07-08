using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffEffect : MonoBehaviour, IEffect
{
    public StatModStatusCondition buff;

    public void Resolve(UnitCreature emitter, UnitCreature receiver)
    {
        emitter.AddStatusCondition(Instantiate(buff));
    }
}
