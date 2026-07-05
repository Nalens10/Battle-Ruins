using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MessageTag
{
    NEXT_TURN,
    ITEMSKILL_MISS,
    ITEMSKILL_DAMAGE,

    UNITCREATURE_SELECTED,
    UNITCREATURE_UPDATED,

    ACTION_UNITCREATURE_MOVE,
    ACTION_UNITCREATURE_ITEMSKILL,

    GAME_OVER
}

public abstract class Message
{
    public abstract MessageTag tag { get; }
}
