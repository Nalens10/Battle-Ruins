using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


// Les dejo en verde lo que hace ;)

public class UnitCreatureUI : MonoBehaviour, IMessageListener
{
    public GameObject[] energyBlocks;

    public Slider healthSlider;

    public DynamicItemSkillUIList dynStatList;
    public DynamicItemSkillUIList dynButtonList;

    protected UnitCreature selectedCreature;

    void Start()
    {
        MessageManager.current.AddListener(MessageTag.UNITCREATURE_SELECTED, this);
        MessageManager.current.AddListener(MessageTag.UNITCREATURE_UPDATED, this);

        this.dynStatList.ConfigureAndHide();
        this.dynButtonList.ConfigureAndHide();

        this.Hide();
    }

    public void DisplayStats(Stats stats)
    {
        this.DisplayEnergy(stats.energy);

        this.healthSlider.value = stats.hp / (float)stats.maxhp;

        this.dynStatList.GetNextItemAndActivate<SingleStatUI>().Configure("Atk", stats.attack);
        this.dynStatList.GetNextItemAndActivate<SingleStatUI>().Configure("EAtk", stats.elemAttack);
        this.dynStatList.GetNextItemAndActivate<SingleStatUI>().Configure("Def", stats.defense);
        this.dynStatList.GetNextItemAndActivate<SingleStatUI>().Configure("EDef", stats.elemDefense);
        this.dynStatList.GetNextItemAndActivate<SingleStatUI>().Configure("Spd", stats.speed);
        this.dynStatList.GetNextItemAndActivate<SingleStatUI>().Configure("Acc", stats.accuracy);
        this.dynStatList.GetNextItemAndActivate<SingleStatUI>().Configure("Eva", stats.evasion);

    }

    public void DisplayEnergy(int energy)
    {
        foreach (var block in this.energyBlocks)
        {
            block.SetActive(false);
        }

        for (int i = 0; i < energy; i++)
        {
            this.energyBlocks[i].SetActive(true);
        }
    }

    public void AddItemSkillButtton(string itemSkillName, UnityAction onClick)
    {
        ItemSkillButton btn = this.dynButtonList.GetNextItemAndActivate<ItemSkillButton>();
        btn.Configure(itemSkillName, onClick);
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);

        this.dynStatList.HideAll();
        this.dynButtonList.HideAll();
    }

    public void Receive(Message msg)
    {
        if (msg is UnitCreatureSelectedMessage)
        {
            UnitCreatureSelectedMessage csm = msg as UnitCreatureSelectedMessage;

            this.dynButtonList.HideAll();
            this.dynStatList.HideAll();

            this.selectedCreature = csm.unitCreature;
            if (this.selectedCreature != null)
            {
                this.Show();
                this.DisplayStats(this.selectedCreature.GetCurrentStats());

                if (this.selectedCreature.master is PlayerMaster)
                {
                    ItemSkill[] skills = this.selectedCreature.GetItemSkills();

                    this.AddItemSkillButtton("Move", () =>
                    {
                        MessageManager.current.Send(
                            new UnitCreatureActionMoveMessage(this.selectedCreature)
                        );
                    });

                    foreach (var skill in skills)
                    {
                        this.AddItemSkillButtton(skill.name, () =>
                        {
                            MessageManager.current.Send(
                                new UnitCreatureActionItemSkillMessage(this.selectedCreature, skill)
                            );
                        });
                    }
                }
            }
            else
            {
                this.Hide();
            }
        }


        if (msg is UnitCreatureUpdatedMessage)
        {
            UnitCreatureUpdatedMessage cm = msg as UnitCreatureUpdatedMessage;

            if (this.selectedCreature == cm.unitCreature)
            {
                this.dynStatList.HideAll();
                this.DisplayStats(this.selectedCreature.GetCurrentStats());
            }
        }
    }
}
