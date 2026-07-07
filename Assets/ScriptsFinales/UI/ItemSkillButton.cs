using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemSkillButton : MonoBehaviour
{
    public TextMeshProUGUI label;
    public TextMeshProUGUI itemSkillCost;
    public Button btn;

    public void Configure(string text, UnityAction onClick, ItemSkill itemSkill)
    {
        this.label.text = text;

        this.btn.onClick.RemoveAllListeners();
        this.btn.onClick.AddListener(onClick);

        if (itemSkill != null)
        {
            this.itemSkillCost.gameObject.SetActive(true);
            this.itemSkillCost.text = "Cost: " + itemSkill.cost.ToString();

        }
        else
        {
            this.itemSkillCost.gameObject.SetActive(false);
        }
    }
}
