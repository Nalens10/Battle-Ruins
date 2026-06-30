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

    public void SetColourUsingPathCost(int cost)
    {
        this.Hide();

        this.markerSprites[cost % this.markerSprites.Length].SetActive(true);
    }

    public void Hide()
    {
        foreach (var spr in this.markerSprites)
        {
            spr.SetActive(false);
        }
    }
}
