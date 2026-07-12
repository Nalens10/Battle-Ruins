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

        if (label != null)
            label.text = text;

        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(onClick);
        }

        if (itemSkillCost != null)
        {
            if (skill != null)
            {
                itemSkillCost.gameObject.SetActive(true);
                itemSkillCost.text = "Cost: " + skill.cost;
            }
            else
            {
                itemSkillCost.gameObject.SetActive(false);
            }
        }
    }

    public void Configure(string text, UnityAction onClick, ItemInstance item)
    {
        itemInstance = item;
        itemSkill = item != null ? item.itemSkill : null;

        if (label != null)
            label.text = text;

        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(onClick);
        }

        if (itemSkillCost != null)
        {
            if (item != null && item.itemSkill != null)
            {
                itemSkillCost.gameObject.SetActive(true);
                itemSkillCost.text =
                    "Cost: " + item.itemSkill.cost +
                    "\nUses: " + item.remainingUses;
            }
            else
            {
                itemSkillCost.gameObject.SetActive(false);
            }
        }
    }

    public void ConfigureReplace(string text, UnityAction onClick)
    {
        itemSkill = null;
        itemInstance = null;

        label.text = text;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(onClick);

        if (itemSkillCost != null)
            itemSkillCost.gameObject.SetActive(false);
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
            return;

        if (itemInstance != null)
            ItemViewerManager.current.Show(itemInstance);
        else if (itemSkill != null)
            ItemViewerManager.current.Show(itemSkill);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (ItemViewerManager.current != null)
            ItemViewerManager.current.Hide();
    }
}
