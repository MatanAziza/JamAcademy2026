using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Panneaux UI")]
    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI;

    [Header("Réglages")]
    public Slider volumeSlider;

    private bool isPaused = false;

    [Header("Death Settings")]
    public GameObject deathMenuPanel;

    public void TriggerDeath()
    {
        deathMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }


    void Start()
    {

        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (settingsMenuUI != null) settingsMenuUI.SetActive(false);

        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused && settingsMenuUI.activeSelf)
                CloseSettings();
            else if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false); 
        settingsMenuUI.SetActive(false); 
        Time.timeScale = 1f;          
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);  
        Time.timeScale = 0f;          
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void QuitGame()
    {
        Application.Quit(); 
    }

    public void OpenSettings()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    public void SetVolume(float volume)
    {
        // Modifie le volume global de tout le jeu (0 = muet, 1 = max)
        AudioListener.volume = volume; 
    }
}