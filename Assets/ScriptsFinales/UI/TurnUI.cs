using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


// Les dejo en verde lo que hace ;)
public class TurnUI : MonoBehaviour, IMessageListener
{
    public TextMeshProUGUI currentTurnLabel;

    public GameObject nextTurnButton;

    public Button moveButton;

    public UnitCreatureUI unitCreatureUI;

    void Start()
    {
        MessageManager.current.AddListener(MessageTag.NEXT_TURN, this);
    }

    public void Receive(Message msg)
    {
        NextTurnMessage ntm = msg as NextTurnMessage;
        this.currentTurnLabel.text = ntm.currentTurnMaster.masterName;

        if (ntm.currentTurnMaster is PlayerMaster)
        {
            this.nextTurnButton.SetActive(true);
        }
        else
        {
            this.nextTurnButton.SetActive(false);
        }
    }

    public void OnMoveButton()
    {
        PlayerMaster player = GameManager.current.CurrentPlayer;

        if (player == null)
            return;

        if (!player.hasUnitCreatureSelected)
            return;

        MessageManager.current.Send(
            new UnitCreatureActionMoveMessage(player.selectedUnitCreature)
        );
    }

    public void OnSkillButton()
    {
        PlayerMaster player = GameManager.current.CurrentPlayer;

        if (player == null)
            return;

        if (!player.hasUnitCreatureSelected)
            return;

        unitCreatureUI.RefreshUniqueSkill();
    }

    public void OnInventoryButton()
    {
        PlayerMaster player = GameManager.current.CurrentPlayer;

        if (player == null)
            return;

        if (!player.hasUnitCreatureSelected)
            return;

        unitCreatureUI.RefreshInventory();
    }
}
