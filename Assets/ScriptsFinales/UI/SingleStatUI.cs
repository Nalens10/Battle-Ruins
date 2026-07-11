using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SingleStatUI : MonoBehaviour
{
    public TextMeshProUGUI label;
    public TextMeshProUGUI baseValueLabel;
    public TextMeshProUGUI modValueLabel;

    public void Configure(string text, int baseValue, int value)
    {
        this.label.text = text;
        this.baseValueLabel.text = baseValue.ToString();

        int diff = value - baseValue;

        if (diff != 0)
        {
            this.modValueLabel.gameObject.SetActive(true);

            if (diff > 0)
            {
                this.modValueLabel.text = "+" + diff.ToString();
            }
            else if (diff < 0)
            {
                this.modValueLabel.text = diff.ToString();
            }
        }
        else
        {
            this.modValueLabel.gameObject.SetActive(false);
        }
    }
}