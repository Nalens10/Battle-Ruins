using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Les dejo en verde lo que hace ;)
public class MapPathMarker : MonoBehaviour
{
    public GameObject[] markerSprites;

    public bool visible { get; protected set; }

    void Awake()
    {
        this.Hide();
    }

    public void SetColourUsingPathCost(int cost)
    {
        this.Hide();

        this.markerSprites[cost % this.markerSprites.Length].SetActive(true);

        this.visible = true;
    }

    public void Hide()
    {
        foreach (var spr in this.markerSprites)
        {
            spr.SetActive(false);
        }
        this.visible = false;
    }
}
