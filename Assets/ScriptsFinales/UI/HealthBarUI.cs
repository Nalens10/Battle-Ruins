using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;  

public class HealthBarUI : MonoBehaviour
{
    public Slider healthSlider;
    public TextMeshProUGUI healthValuesLabel;

    public void SetHealth(int current, int max)
    {
        this.healthSlider.value = current / (float)max;
        this.healthValuesLabel.text = current + " / " + max;
    }
}
