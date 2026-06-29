using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Les dejo en verde lo que hace ;)

public class UnitCreatureUI : MonoBehaviour
{
    
    public static UnitCreatureUI current;

    public GameObject[] energyBlocks;

    void Awake()
    {
        current = this;
        this.Hide();
    }

    public void DisplayEnergy(int energy)
    {
        // Oculta todos los bloques de energía
        foreach (var block in this.energyBlocks)
        {
            block.SetActive(false);
        }
        // Muestra los bloques de energía correspondientes a la cantidad de energía actual
        for (int i = 0; i < energy; i++)
        {
            this.energyBlocks[i].SetActive(true);
        }
    }

    // Muestra el UI de la criatura
    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    // Oculta el UI de la criatura
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
