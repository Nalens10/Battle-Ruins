using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSkillFeedbackManager : MonoBehaviour, IMessageListener
{
    private List<ItemSkillFeedbackUI> items;

    void Start()
    {
        MessageManager.current.AddListener(MessageTag.ITEMSKILL_DAMAGE, this);
        MessageManager.current.AddListener(MessageTag.ITEMSKILL_MISS, this);

        this.items = new List<ItemSkillFeedbackUI>();

        int childCount = this.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = this.transform.GetChild(i);
            ItemSkillFeedbackUI feedbackUI = child.GetComponent<ItemSkillFeedbackUI>();
            feedbackUI.Hide();

            this.items.Add(feedbackUI);
        }
    }

    public void Receive(Message msg)
    {
        if (msg is ItemSkillDamageMessage)
        {
            ItemSkillDamageMessage sm = msg as ItemSkillDamageMessage;
            ItemSkillFeedbackUI ui = this.GetNextItemForReceiver(sm.receiver);

            ui.ConfigureForDamage(sm.receiver, sm.damage, sm.critical);
        }

        if (msg is ItemSkillMissMessage)
        {
            ItemSkillMissMessage sm = msg as ItemSkillMissMessage;
            ItemSkillFeedbackUI ui = this.GetNextItemForReceiver(sm.receiver);

            ui.ConfigureForMiss(sm.receiver);
        }
    }

    public ItemSkillFeedbackUI GetNextItemForReceiver(UnitCreature receiver)
    {
        foreach (var fui in this.items)
        {
            if (fui.isHidden || fui.receiver == receiver)
            {
                return fui;
            }
        }

        GameObject sample = this.items[0].gameObject;
        GameObject copy = Instantiate(sample);

        copy.transform.SetParent(this.transform);

        ItemSkillFeedbackUI newui = copy.GetComponent<ItemSkillFeedbackUI>();
        this.items.Add(newui);
        newui.Hide();

        return newui;
    }
}
