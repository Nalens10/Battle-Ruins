using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectorPlayer : MonoBehaviour
{
    [SerializeField] private int playerID; //0,1,2,3

    [SerializeField] private string playerName;

    [SerializeField] private Toggle toggle;

    [SerializeField] private Image imagen;

    [SerializeField] private List<Character> characters;

    private int indexCharacter;

    private void Start()
    {
        PlayerData data = CharacterSelection.Instance.players[playerID];

        data.playerName = playerName;

        data.isPlaying = toggle.isOn;

        indexCharacter = 0;

        data.selectedCharacter = characters[indexCharacter];

        UpdateCharacterImage();

        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void UpdateCharacterImage()
    {
        imagen.sprite = characters[indexCharacter].characterSprite;

        CharacterSelection.Instance.players[playerID].selectedCharacter =
            characters[indexCharacter];
    }

    public void NextCharacter()
    {
        indexCharacter++;

        if (indexCharacter >= characters.Count)
            indexCharacter = 0;

        UpdateCharacterImage();
    }

    public void PreviousCharacter()
    {
        indexCharacter--;

        if (indexCharacter < 0)
            indexCharacter = characters.Count - 1;

        UpdateCharacterImage();
    }

    public void OnToggleChanged(bool value)
    {
        CharacterSelection.Instance.players[playerID].isPlaying = value;
    }
}
