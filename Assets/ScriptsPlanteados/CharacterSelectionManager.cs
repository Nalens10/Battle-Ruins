using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionManager : MonoBehaviour
{
    public List<CharacterData> availableCharacters;

    public List<CharacterData> selectedCharacters =
        new List<CharacterData>();

    public int maxPlayers = 4;

    public void SelectCharacter(CharacterData character)
    {
        if (selectedCharacters.Count >= maxPlayers)
            return;

        if (!selectedCharacters.Contains(character))
        {
            selectedCharacters.Add(character);
        }
    }

    public void ConfirmSelection()
    {
        SendSelectionToGameManager();
    }

    public void SendSelectionToGameManager()
    {
        GameManager.Instance.CreatePlayers(selectedCharacters);
    }
}
