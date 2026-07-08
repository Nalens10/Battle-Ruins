using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public ItemSkill itemSkill;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetItem(ItemSkill skill)
    {
        itemSkill = skill;

        spriteRenderer.sprite = skill.worldSprite;
    }
}
