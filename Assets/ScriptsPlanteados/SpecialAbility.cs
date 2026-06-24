using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialAbility : ScriptableObject
{
    public string abilityName;

    [TextArea]
    public string description;

    public int cooldown;

    public abstract void Activate(PlayerController player);

    protected virtual void ApplyEffect(PlayerController player)
    {
    }
}
