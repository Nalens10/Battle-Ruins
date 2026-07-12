using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;   

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
    [SerializeField] private Slider volumeSlider;
    private float ultimoVolumen = 0f;
    private bool muteado = false;

    public void FullScreen(bool pantallaCompleta)
    {
        Debug.Log("Cambio de Pantalla...");
        Screen.fullScreen = pantallaCompleta;
    }

    public void ChangeVolume(float volumen)
    {
        Debug.Log("Cambio de Volumen...");
        ultimoVolumen = volumen;
        audioMixer.SetFloat("Volumen", volumen);
    }

    public void ChangeQuality(int calidad)
    {
        Debug.Log("Cambio de Calidad...");
        QualitySettings.SetQualityLevel(calidad);
    }

    public void ChangeMute()
    {
        if (!muteado)
        {
            audioMixer.GetFloat("Volumen", out ultimoVolumen);
            audioMixer.SetFloat("Volumen", -80f);
        }
        else
        {
            audioMixer.SetFloat("Volumen", ultimoVolumen);
        }

        muteado = !muteado;
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
