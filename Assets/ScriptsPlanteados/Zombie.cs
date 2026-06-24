using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public int health = 50;
    public int damage = 15;

    public void Move()
    {
    }

    public void Attack(PlayerController target)
    {
        target.TakeDamage(damage);
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
