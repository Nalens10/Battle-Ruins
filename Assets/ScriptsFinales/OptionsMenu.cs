using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
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

}
