using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public int currentPlayer;
    public int currentRound;

    public float turnTime;

    public Timer timer;

    public void StartTurn()
    {
        Debug.Log("Turno de: " +
            GameManager.Instance.players[currentPlayer].playerName);

        timer.ResetTimer(turnTime);
        timer.StartTimer();
    }

    public void EndTurn()
    {
        timer.StopTimer();

        currentPlayer++;

        if (currentPlayer >= GameManager.Instance.players.Count)
        {
            currentPlayer = 0;
            NextRound();
        }

        StartTurn();
    }

    public void NextRound()
    {
        currentRound++;

        Debug.Log("Ronda " + currentRound);
    }

}
