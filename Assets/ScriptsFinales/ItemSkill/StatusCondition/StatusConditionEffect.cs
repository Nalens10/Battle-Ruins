using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusConditionEffect : MonoBehaviour, IEffect
{
    protected StatusCondition[] conditions;

    void Awake()
    {
        this.conditions = this.GetComponentsInChildren<StatusCondition>();

        foreach (var cond in this.conditions)
        {
            if (cond.gameObject == this.gameObject)
            {
                Debug.LogError("Las condiciones de estado deben estar en un GameObject diferente al del Item/Skill");
            }
        }
    }

    public void Resolve(UnitCreature emitter, UnitCreature receiver)
    {
        foreach (var cond in conditions)
        {
            GameObject parasiteObj = Instantiate(cond.gameObject);

            StatusCondition condition =
                parasiteObj.GetComponent<StatusCondition>();

            receiver.AddStatusCondition(condition);
        }
    }
}