using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


// Les dejo en verde lo que hace ;)

public class UnitCreatureUI : MonoBehaviour, IMessageListener
{
    public GameObject[] energyBlocks;

    public HealthBarUI healthBar;

    public TextMeshProUGUI elementalTypeLabel;

    public DynamicItemSkillUIList dynButtonList;
    public DynamicItemSkillUIList dynStatList;

    public StatusConditionListUI statusConditionListUI;

    protected UnitCreature selectedUnitCreature;

    void Start()
    {
        MessageManager.current.AddListener(MessageTag.UNITCREATURE_SELECTED, this);
        MessageManager.current.AddListener(MessageTag.UNITCREATURE_UPDATED, this);

        this.dynButtonList.ConfigureAndHide();
        this.dynStatList.ConfigureAndHide();

        this.Hide();
    }

    public void DisplayStats(Stats baseStats, Stats currentStats)
    {
        this.elementalTypeLabel.text = baseStats.elementalType.ToString();

        this.DisplayEnergy(currentStats.energy);

        this.healthBar.SetHealth(currentStats.hp, currentStats.maxhp);

        this.dynStatList.GetNextItemAndActivate<SingleStatUI>().Configure("Atk", baseStats.attack, currentStats.attack);
        this.dynStatList.GetNextItemAndActivate<SingleStatUI>().Configure("EAtk", baseStats.elemAttack, currentStats.elemAttack);
        this.dynStatList.GetNextItemAndActivate<SingleStatUI>().Configure("Def", baseStats.defense, currentStats.defense);
        this.dynStatList.GetNextItemAndActivate<SingleStatUI>().Configure("EDef", baseStats.elemDefense, currentStats.elemDefense);
        this.dynStatList.GetNextItemAndActivate<SingleStatUI>().Configure("Spd", baseStats.speed, currentStats.speed);
        this.dynStatList.GetNextItemAndActivate<SingleStatUI>().Configure("Acc", baseStats.accuracy, currentStats.accuracy);
        this.dynStatList.GetNextItemAndActivate<SingleStatUI>().Configure("Eva", baseStats.evasion, currentStats.evasion);
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

    public void AddItemSkillButtton(string itemSkillName, ItemSkill itemSkill, UnityAction onClick)
    {
        ItemSkillButton btn = this.dynButtonList.GetNextItemAndActivate<ItemSkillButton>();
        btn.Configure(itemSkillName, onClick, itemSkill);
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);

        this.dynButtonList.HideAll();
        this.dynStatList.HideAll();
    }

    public void Receive(Message msg)
    {
        if (msg is UnitCreatureSelectedMessage)
        {
            UnitCreatureSelectedMessage csm = msg as UnitCreatureSelectedMessage;

            this.dynButtonList.HideAll();
            this.dynStatList.HideAll();

            this.selectedUnitCreature = csm.unitCreature;
            if (this.selectedUnitCreature != null)
            {
                this.Show();
                this.DisplayStats(this.selectedUnitCreature.GetBaseStats(), this.selectedUnitCreature.GetCurrentStats());
                this.statusConditionListUI.DisplayStatusConditions(this.selectedUnitCreature.GetCurrentStatusConditions());

                if (this.selectedUnitCreature.master is PlayerMaster)
                {
                    ItemSkill[] itemSkills = this.selectedUnitCreature.GetItemSkills();

                    // Agregar botones para cada item skill

                    foreach (var itemSkill in itemSkills)
                    {
                        this.AddItemSkillButtton(itemSkill.name, itemSkill, () =>
                        {
                            MessageManager.current.Send(
                                new UnitCreatureActionItemSkillMessage(this.selectedUnitCreature, itemSkill)
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

            if (this.selectedUnitCreature == cm.unitCreature)
            {
                this.dynStatList.HideAll();
                this.DisplayStats(this.selectedUnitCreature.GetBaseStats(), this.selectedUnitCreature.GetCurrentStats());
                this.statusConditionListUI.DisplayStatusConditions(this.selectedUnitCreature.GetCurrentStatusConditions());
            }
        }
    }
}
