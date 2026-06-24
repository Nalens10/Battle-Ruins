using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    public Weapon currentWeapon;

    public void EquipWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
    }

    public void FireWeapon(PlayerController target)
    {
        if (currentWeapon == null)
            return;

        currentWeapon.UseWeapon(target);
    }

    public int CalculateDamage()
    {
        return currentWeapon.damage;
    }
}
