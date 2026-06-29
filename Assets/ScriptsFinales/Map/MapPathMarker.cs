using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Les dejo en verde lo que hace ;)
public class MapPathMarker : MonoBehaviour
{
    public GameObject[] markerSprites;

    void Awake()
    {
        this.Hide();
    }

    // Muestra el marcador de ruta correspondiente al costo de la ruta
    public void SetColourUsingPathCost(int cost)
    {
        this.Hide();

        this.markerSprites[cost % this.markerSprites.Length].SetActive(true);
    }

    // Oculta todos los marcadores de ruta
    public void Hide()
    {
        foreach (var spr in this.markerSprites)
        {
            spr.SetActive(false);
        }
    }
}
