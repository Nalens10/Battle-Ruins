using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : ScriptableObject
{
    public string weaponName;

    public int damage;

    public int range;

    public abstract void UseWeapon(PlayerController target);

    protected void ApplyDamage(PlayerController target)
    {
        target.TakeDamage(damage);
    }
}
