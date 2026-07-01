using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ElementalWeaknessDB
{
    static Dictionary<(ElementalType, ElementalType), float> db;

    static ElementalWeaknessDB()
    {
        db = new Dictionary<(ElementalType, ElementalType), float>();

        // FIRE
        db.Add((ElementalType.FIRE, ElementalType.ICE), 1.5f);
        db.Add((ElementalType.FIRE, ElementalType.POISON), 1.5f);
        db.Add((ElementalType.FIRE, ElementalType.WATER), 0.5f);

        // WATER
        db.Add((ElementalType.WATER, ElementalType.FIRE), 1.5f);
        db.Add((ElementalType.WATER, ElementalType.POISON), 1.5f);
        db.Add((ElementalType.WATER, ElementalType.LIGHTNING), 0.5f);

        // LIGHTNING
        db.Add((ElementalType.LIGHTNING, ElementalType.WATER), 1.5f);
        db.Add((ElementalType.LIGHTNING, ElementalType.ICE), 1.5f);
        db.Add((ElementalType.LIGHTNING, ElementalType.POISON), 0.5f);

        // ICE
        db.Add((ElementalType.ICE, ElementalType.LIGHTNING), 1.5f);
        db.Add((ElementalType.ICE, ElementalType.POISON), 1.5f);
        db.Add((ElementalType.ICE, ElementalType.FIRE), 0.5f);

        // POISON
        db.Add((ElementalType.POISON, ElementalType.LIGHTNING), 1.5f);
        db.Add((ElementalType.POISON, ElementalType.WATER), 1.5f);
        db.Add((ElementalType.POISON, ElementalType.FIRE), 0.5f);
    }

    public static float GetWeaknessMultiplier(ElementalType skillType, ElementalType receiverType)
    {
        (ElementalType, ElementalType) pair = (skillType, receiverType);

        if (db.ContainsKey(pair))
        {
            return db[pair];
        }

        return 1f;
    }

}
