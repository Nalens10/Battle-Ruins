using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour, IMessageListener
{
    public TextMeshProUGUI winnerLabel;
    public GameObject uiPanel;

    void Start()
    {
        MessageManager.current.AddListener(MessageTag.GAME_OVER, this);
        this.uiPanel.SetActive(false);
    }

    public void Receive(Message msg)
    {
        GameOverMessage bom = msg as GameOverMessage;

        StartCoroutine(this.DisplayWinner(bom));
    }

    private IEnumerator DisplayWinner(GameOverMessage bom)
    {
        yield return new WaitForSeconds(1.2f);

        this.uiPanel.SetActive(true);
        this.winnerLabel.text = bom.winner.masterName + " wins!";
    }

}