using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public Button moveButton;
    public Button attackButton;
    public Button inventoryButton;
    public Button endTurnButton;

    public TMP_Text healthText;
    public TMP_Text turnText;

    public PlayerController currentPlayer;

    public void OnMovePressed()
    {
        Debug.Log("Mover");
    }

    public void OnAttackPressed()
    {
        Debug.Log("Atacar");
    }

    public void OnUseItemPressed()
    {
        Debug.Log("Usar objeto");
    }

    public void OnEndTurnPressed()
    {
        FindObjectOfType<TurnManager>().EndTurn();
    }

    public void UpdateHealthUI()
    {
        healthText.text =
            "HP: " + currentPlayer.health;
    }

    public void UpdateInventoryUI()
    {
    }

    public void UpdateTurnUI()
    {
        turnText.text =
            "Jugador: " + currentPlayer.playerName;
    }

}
