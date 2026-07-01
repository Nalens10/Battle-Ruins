using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

// Les dejo en verde lo que hace ;)
public class UnitCreature : MonoBehaviour
{
    public Vector2Int localPosition;

    public GameObject selectionIndicator;

    public float movementSpeed = 4f;

    private bool isSelected = false;

    public Master master;

    public Stats stats;

    private List<StatusCondition> conditions = new List<StatusCondition>();

    public void Start ()
    {
        this.SetSelectionStatus(false);
    }

    public Stats GetCurrentStats()
    {
        Stats modedStats = this.stats.Clone();

        foreach (var cond in this.conditions)
        {
            cond.ApplyStatsModifiers(modedStats);
        }

        return modedStats;
    }

    public void ModifyHealth(int amount)
    {
        int newHP = this.stats.hp + amount;

        this.stats.hp = Mathf.Clamp(newHP, 0, this.stats.maxhp);
    }

    public void BeginTurn()
    {
        this.UpdateEnergy(this.stats.maxEnergy);

        for (int i = 0; i < this.conditions.Count; i++)
        {
            StatusCondition cond = this.conditions[i];
            cond.ConsumeOneTurn();

            if (cond.isDepleted)
            {
                this.conditions.RemoveAt(i);
            }
        }

        foreach (var cond in this.conditions)
        {
            cond.ApplyOnTurnStart(this.stats);
        }
    }

    public void AddStatusCondition(StatusCondition condition)
    {
        this.conditions.Add(condition);

        condition.transform.position = this.transform.position;
        condition.transform.SetParent(this.transform);
    }

    public int CurrentMaxDistance()
    {
        return this.stats.speed * this.stats.energy;
    }

    private void UpdateEnergy(int e)
    {
        this.stats.energy = e;
        MessageManager.current.Send(new UnitCreatureUpdatedMessage(this));
    }

    public bool CanExecuteItemSkill(ItemSkill skill)
    {
        return this.stats.energy >= skill.cost;
    }

    public void ConsumeEnergyFor(ItemSkill skill)
    {
        this.UpdateEnergy(this.stats.energy - skill.cost);
    }

    public void SetSelectionStatus(bool isSelected)
    {
        this.selectionIndicator.SetActive(isSelected);
        this.isSelected = isSelected;
    }

    public void FollowPath(Vector3[] worldPath)
    {
        StopAllCoroutines();
        StartCoroutine(this.FollowPathRutine(worldPath));
    }

    private IEnumerator FollowPathRutine(Vector3[] worldPath)
    {
        int pathLength = Mathf.Min(this.CurrentMaxDistance(), worldPath.Length);
        int cost = this.GetEnergyCostForPathLength(pathLength);

        this.UpdateEnergy(this.stats.energy - cost);

        for (int i = 0; i < pathLength; i++)
        {
            Vector3 target = worldPath[i];

            float percent = 0;

            Vector3 start = this.transform.position;

            while (percent < 1f)
            {
                this.transform.position = Vector3.Lerp(start, target, percent);

                percent += Time.deltaTime * this.movementSpeed;
                yield return null;
            }

            this.transform.position = target;
        }
    }

    public int GetEnergyCostForPathLength(int length)
    {
        int cost = Mathf.CeilToInt(length / (float)this.stats.speed);

        return cost;
    }

    public ItemSkill[] GetItemSkills()
    {
        return this.GetComponentsInChildren<ItemSkill>();
    }
}
