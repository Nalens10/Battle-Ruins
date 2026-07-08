using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MessageTag
{
    NEXT_TURN,
    ITEMSKILL_MISS,
    ITEMSKILL_HEALTH_MOD,

    UNITCREATURE_SELECTED,
    UNITCREATURE_UPDATED,
    UNITCREATURE_MOVED,

    ACTION_UNITCREATURE_MOVE,
    ACTION_UNITCREATURE_ITEMSKILL,
    ACTION_UNITCREATURE_INVENTORYITEM,

    GAME_OVER
}

public abstract class Message
{
    public abstract MessageTag tag { get; }
}
