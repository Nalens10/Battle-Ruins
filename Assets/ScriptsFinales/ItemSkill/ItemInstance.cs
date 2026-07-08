using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemInstance
{
    public ItemSkill itemSkill;

    public int remainingUses;

    public bool IsDepleted
    {
        get
        {
            return remainingUses <= 0;
        }
    }

    public bool ConsumeUse()
    {
        remainingUses--;
        return IsDepleted;
    }
}
