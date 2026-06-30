using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats
{
    [Header("HP/Energy")]
    public int hp = 100;
    public int maxhp = 100;

    public int energy = 2;
    public int maxEnergy = 2;

    [Header("Combat")]
    public int attack = 2;
    public int defense = 2;

    [Header("Movement")]
    public int speed = 4;
}
