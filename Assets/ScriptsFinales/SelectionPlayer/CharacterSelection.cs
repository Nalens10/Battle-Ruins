using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    public static CharacterSelection Instance;

    public List<PlayerData> players = new List<PlayerData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (players.Count == 0)
            {
                for (int i = 0; i < 4; i++)
                    players.Add(new PlayerData());
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
