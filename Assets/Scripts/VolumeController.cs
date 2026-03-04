using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeController : MonoBehaviour
{
    [Header("Références")]
    public AudioMixer masterMixer;
    public string parameterName = "MasterVol"; // Le nom qu'on a mis à l'étape 2

    private Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void Start()
    {
        // On récupère le volume sauvegardé, sinon 0.75 par défaut
        float savedVol = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        slider.value = savedVol;
        
        SetVolume(savedVol);
    }

    public void SetVolume(float sliderValue)
    {
        // On convertit la valeur 0-1 du slider en décibels (-80dB à 20dB)
        // La formule Log10 est indispensable pour que le son baisse de façon naturelle
        float dBValue = Mathf.Log10(Mathf.Max(0.0001f, sliderValue)) * 20f;
        
        masterMixer.SetFloat(parameterName, dBValue);
        
        // On sauvegarde pour les prochaines sessions
        PlayerPrefs.SetFloat("MasterVolume", sliderValue);
    }
}