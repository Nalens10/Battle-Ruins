using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class Character : ScriptableObject
{
    public string characterName;

    public GameObject unitCreaturePrfbs;

    public Sprite characterSprite;
}
