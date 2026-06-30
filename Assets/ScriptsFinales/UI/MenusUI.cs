using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MenusUI: MonoBehaviour
{
    // Main Menu
    public void Play()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del Juego...");
        Application.Quit();
    }


    // Options Menu
    [SerializeField] private AudioMixer audioMixer;
    public void FullScreen(bool pantallaCompleta)
    {
        Debug.Log("Cambio de Pantalla...");
        Screen.fullScreen = pantallaCompleta;
    }

    public void ChangeVolume(float volumen)
    {
        Debug.Log("Cambio de Volumen...");
        audioMixer.SetFloat("Volumen", volumen);
    }

    public void ChangeQuality(int calidad)
    {
        Debug.Log("Cambio de Calidad...");
        QualitySettings.SetQualityLevel(calidad);
    }

    // Pause Menu
    public void PauseGame()
    {
        Time.timeScale = 0f;
        Debug.Log("Juego en Pausa...");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        Debug.Log("Juego Reanudado...");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        Debug.Log("Juego Reiniciado...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
