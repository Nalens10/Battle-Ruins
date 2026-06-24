using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject endGameScreen;

    public void ShowPauseMenu()
    {
        pauseMenu.SetActive(true);
    }

    public void ShowEndGameScreen()
    {
        endGameScreen.SetActive(true);
    }
}
