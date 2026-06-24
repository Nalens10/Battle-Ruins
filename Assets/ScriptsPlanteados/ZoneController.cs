using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneController : MonoBehaviour
{
    public float currentRadius = 100;

    public int damagePerRound = 10;

    public void ShrinkZone()
    {
        currentRadius -= 10;
    }

    public void ApplyZoneDamage(PlayerController player)
    {
        player.TakeDamage(damagePerRound);
    }

    public void UpdateZoneRules()
    {
        Debug.Log("Zona actualizada");
    }
}
