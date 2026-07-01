using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectorPlayer : MonoBehaviour
{
    [SerializeField] private int playerID; //0,1,2,3

    public List<Character> characters;

    private int indexCharacter;

    [SerializeField] private Image imagen;

    private void Start()
    {
        indexCharacter = PlayerPrefs.GetInt("PlayerIndex_" + playerID, 0);

        if (indexCharacter >= characters.Count)
        {
            indexCharacter = 0;
        }

        UpdateCharacterImage();
    }

    void UpdateCharacterImage()
    {
        PlayerPrefs.SetInt("PlayerIndex_" + playerID, indexCharacter);

        imagen.sprite = characters[indexCharacter].characterSprite;
    }

    public void NextCharacter()
    {
        Debug.Log("Siguiente");
        indexCharacter++;

        if (indexCharacter >= characters.Count)
            indexCharacter = 0;

        UpdateCharacterImage();
    }

    public void PreviousCharacter()
    {
        Debug.Log("Previo");
        indexCharacter--;

        if (indexCharacter < 0)
            indexCharacter = characters.Count - 1;

        UpdateCharacterImage();
    }
}
