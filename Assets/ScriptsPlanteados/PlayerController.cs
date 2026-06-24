using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerController : MonoBehaviour
{
    [Header("Personaje")]
    public CharacterData characterData;

    [Header("Stats")]
    public int health;
    public int armor;

    public int currentMovement;

    public bool extraAttackAvailable;

    public InventorySystem inventory;
    public WeaponSystem weaponSystem;

    internal string playerName;

    private void Start()
    {
        if (characterData != null)
        {
            health = characterData.maxHealth;
            currentMovement = characterData.movementRange;
        }
    }

    public void Move(Vector3 destination)
    {
        transform.position = destination;
    }

    public void Attack(PlayerController target)
    {
        weaponSystem.FireWeapon(target);
    }

    public void UseItem(Item item)
    {
        item.Use(this);
    }

    public void UseSpecialAbility()
    {
        characterData.UseSpecialAbility(this);
    }

    public void TakeDamage(int damage)
    {
        int finalDamage = Mathf.Max(0, damage - armor);

        health -= finalDamage;

        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    public void Heal(int amount)
    {
        health += amount;

        if (health > characterData.maxHealth)
            health = characterData.maxHealth;
    }

    private void Die()
    {
        Debug.Log(name + " murió");

        GameManager.Instance.CheckVictoryCondition();
    }

    public bool IsAlive()
    {
        return health > 0;
    }
}
