using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemViewerManager : MonoBehaviour
{
    public Image icon;

    public TMP_Text nameItem;
    public TMP_Text elementItem;
    public TMP_Text statusConditionItem;
    public TMP_Text damageItem;
    public TMP_Text usesItem;
    public TMP_Text costItem;

    public static ItemViewerManager current;

    void Awake()
    {
        current = this;
        Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show(ItemInstance item)
    {
        Debug.Log("CURRENT = " + current);
        Debug.Log("ICON = " + icon);
        Debug.Log("NAME = " + nameItem);
        Debug.Log("ELEMENT = " + elementItem);
        Debug.Log("STATUS = " + statusConditionItem);
        Debug.Log("DAMAGE = " + damageItem);
        Debug.Log("USES = " + usesItem);
        Debug.Log("COST = " + costItem);


        gameObject.SetActive(true);

        ItemSkill skill = item.itemSkill;

        icon.sprite = skill.worldSprite;

        nameItem.text = skill.itemSkillName;

        costItem.text = skill.cost.ToString();

        usesItem.text = item.remainingUses.ToString();

        FillElement(skill);

        FillDamage(skill);

        FillStatus(skill);
    }

    public void Show(ItemSkill skill)
    {
        Debug.Log("CURRENT = " + current);
        Debug.Log("ICON = " + icon);
        Debug.Log("NAME = " + nameItem);
        Debug.Log("ELEMENT = " + elementItem);
        Debug.Log("STATUS = " + statusConditionItem);
        Debug.Log("DAMAGE = " + damageItem);
        Debug.Log("USES = " + usesItem);
        Debug.Log("COST = " + costItem);


        gameObject.SetActive(true);

        icon.sprite = skill.worldSprite;

        nameItem.text = skill.itemSkillName;

        costItem.text = skill.cost.ToString();

        usesItem.text = "∞";

        FillElement(skill);

        FillDamage(skill);

        FillStatus(skill);
    }

    private void FillElement(ItemSkill skill)
    {
        if (skill.elementalType == ElementalType.NONE)
        {
            elementItem.text = "-";
        }
        else
        {
            elementItem.text = skill.elementalType.ToString();
        }
    }

    private void FillDamage(ItemSkill skill)
    {
        DamageEffect damage = skill.GetComponent<DamageEffect>();

        if (damage == null)
        {
            damageItem.text = "-";
            return;
        }

        damageItem.text = damage.power.ToString();
    }

    private void FillStatus(ItemSkill skill)
    {
        StatusCondition[] conditions =
            skill.GetComponentsInChildren<StatusCondition>();

        if (conditions.Length == 0)
        {
            statusConditionItem.text = "-";
            return;
        }

        statusConditionItem.text = "";

        for (int i = 0; i < conditions.Length; i++)
        {
            statusConditionItem.text += conditions[i].conditionName;

            if (i < conditions.Length - 1)
                statusConditionItem.text += ", ";
        }
    }

}
