using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSkillButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public TextMeshProUGUI label;
    public TextMeshProUGUI itemSkillCost;
    public Button btn;

    private ItemSkill itemSkill;
    private ItemInstance itemInstance;

    public void Configure(string text, UnityAction onClick, ItemSkill skill)
    {
        itemSkill = skill;
        itemInstance = null;

        label.text = text;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(onClick);

        itemSkillCost.gameObject.SetActive(true);
        itemSkillCost.text = "Cost: " + skill.cost;
    }

    public void Configure(string text, UnityAction onClick, ItemInstance item)
    {
        itemInstance = item;
        itemSkill = item.itemSkill;

        label.text = text;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(onClick);

        itemSkillCost.gameObject.SetActive(true);
        itemSkillCost.text =
            "Cost: " + item.itemSkill.cost +
            "\nUses: " + item.remainingUses;
    }

    public void ConfigureUniqueSkill(string text, UnityAction onClick, UniqueItemSkill skill, UnitCreature owner)
    {
        itemSkill = skill;
        itemInstance = null;

        label.text = text;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(onClick);

        itemSkillCost.gameObject.SetActive(true);

        if (owner.CanUseUniqueSkill())
        {
            itemSkillCost.text =
                "Cost: " + skill.cost +
                "\nReady";

            btn.interactable = true;
        }
        else
        {
            itemSkillCost.text =
                "Cooldown: " + owner.currentUniqueCooldown;

            btn.interactable = false;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("SELECT");

        Debug.Log(ItemViewerManager.current);

        if (ItemViewerManager.current == null)
        {
            Debug.LogError("ItemViewerManager.current es NULL");
            return;
        }

        if (itemInstance != null)
            ItemViewerManager.current.Show(itemInstance);
        else if (itemSkill != null)
            ItemViewerManager.current.Show(itemSkill);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ItemViewerManager.current.Hide();
    }
}
