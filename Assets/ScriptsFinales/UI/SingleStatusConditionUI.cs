using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SingleStatusConditionUI : MonoBehaviour
{
    public TextMeshProUGUI statusConditionLabel;
    public TextMeshProUGUI statusConditionTurnsLabel;

    public void Configure(StatusCondition condition)
    {
        this.statusConditionLabel.text = condition.conditionName;
        this.statusConditionTurnsLabel.text = condition.remainingTurns.ToString();
    }
}
