using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverMessage : Message
{
    public override MessageTag tag => MessageTag.GAME_OVER;

    public Master winner { get; protected set; }
    public Master losser { get; protected set; }

    public GameOverMessage(Master winner, Master losser)
    {
        this.winner = winner;
        this.losser = losser;
    }
}
