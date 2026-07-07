using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapActionMarker : MonoBehaviour
{
    public GameObject itemSkillReachMarkerSprite;
    public GameObject itemSkillAttackMarkerSprite;

    public GameObject[] pathMarkerSprites;

    public bool visible { get; protected set; }

    void Awake()
    {
        this.Hide();
    }

    public void ShowForItemSkillReach()
    {
        this.Hide();

        this.itemSkillReachMarkerSprite.SetActive(true);

        this.visible = true;
    }

    public void ShowForItemSkillAction()
    {
        this.Hide();

        this.itemSkillAttackMarkerSprite.SetActive(true);

        this.visible = true;
    }

    public void ShowForPathUsingCost(int cost)
    {
        this.Hide();

        this.pathMarkerSprites[cost % this.pathMarkerSprites.Length].SetActive(true);

        this.visible = true;
    }

    public void Hide()
    {
        this.itemSkillReachMarkerSprite.SetActive(false);
        this.itemSkillAttackMarkerSprite.SetActive(false);

        foreach (var spr in this.pathMarkerSprites)
        {
            spr.SetActive(false);
        }

        this.visible = false;
    }
}
