using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


 // Les dejo en verde lo que hace ;)
public class TurnUI : MonoBehaviour
{
    public static TurnUI current;
    
    public TextMeshProUGUI currentTurnLabel;
    void Awake()
    {
        current = this;
    }

    public void SetCurrentTurnLabel(string name)
    {
        // Actualiza el texto del label para mostrar el nombre del jugador actual
        this.currentTurnLabel.text = name;
    }
}
