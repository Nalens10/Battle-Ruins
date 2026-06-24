using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Jugadores")]
    public List<PlayerController> players = new List<PlayerController>();

    [Header("Sistemas")]
    public TurnManager turnManager;

    [Header("Prefabs")]
    public GameObject playerPrefab;

    private bool gameStarted = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void StartGame()
    {
        gameStarted = true;

        Debug.Log("Partida iniciada");

        turnManager.StartTurn();
    }

    public void EndGame(PlayerController winner)
    {
        gameStarted = false;

        Debug.Log("Ganador: " + winner.characterData.characterName);
    }

    public void CheckVictoryCondition()
    {
        int alivePlayers = 0;
        PlayerController winner = null;

        foreach (PlayerController player in players)
        {
            if (player.IsAlive())
            {
                alivePlayers++;
                winner = player;
            }
        }

        if (alivePlayers <= 1)
        {
            EndGame(winner);
        }
    }

    public void CreatePlayers(List<CharacterData> selectedCharacters)
    {
        players.Clear();

        foreach (CharacterData character in selectedCharacters)
        {
            GameObject playerObj = Instantiate(playerPrefab);

            PlayerController player =
                playerObj.GetComponent<PlayerController>();

            player.characterData = character;

            players.Add(player);
        }

        StartGame();
    }
}
