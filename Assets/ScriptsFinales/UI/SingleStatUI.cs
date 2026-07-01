using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SingleStatUI : MonoBehaviour
{
    public TextMeshProUGUI label;
    public TextMeshProUGUI valueLabel;

    public void Configure(string text, float value)
    {
        this.label.text = text;
        this.valueLabel.text = value.ToString();
    }
}
