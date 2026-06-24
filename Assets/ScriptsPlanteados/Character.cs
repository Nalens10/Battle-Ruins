using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Información")]
    public string characterName;
    public Sprite characterPortrait;

    [Header("Estadísticas")]
    public int maxHealth;
    public int baseWeaponDamage;
    public int movementRange;

    [Header("Habilidad")]
    public SpecialAbility specialAbility;

    public virtual void Move()
    {
        Debug.Log(characterName + " se mueve.");
    }

    public virtual void ReceiveDamage(ref int currentHealth, int damage)
    {
        currentHealth -= damage;

        if (currentHealth < 0)
            currentHealth = 0;
    }

    public virtual void Heal(ref int currentHealth, int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    public virtual void UseSpecialAbility(PlayerController player)
    {
        if (specialAbility != null)
        {
            specialAbility.Activate(player);
        }
    }
}
