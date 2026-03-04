using UnityEngine;
using UnityEngine.SceneManagement; 

public class MainMenuManager : MonoBehaviour
{
    [Header("Configuration des Scènes")]
    public string gameSceneName = "Level_01"; 
    public string tutorialSceneName = "Tutorial_Level"; // <-- Ajout pour le tuto

    [Header("Panneaux UI")]
    public GameObject mainMenuPanel; // Le conteneur de tes boutons Jouer/Tuto/Quitter
    public GameObject settingsPanel; // Le conteneur de tes paramètres (Rebind, volume...)

    private void Start()
    {
        // Au lancement, on force l'affichage du menu principal et on cache les paramètres
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    // --- FONCTIONS DE JEU ---
    public void PlayGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(gameSceneName);
    }

    public void PlayTutorial()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(tutorialSceneName);
    }

    // --- FONCTIONS DES PARAMÈTRES ---
    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false); // On cache l'accueil
        settingsPanel.SetActive(true);  // On affiche les paramètres
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false); // On cache les paramètres
        mainMenuPanel.SetActive(true);  // On réaffiche l'accueil
    }

    // --- FONCTION QUITTER ---
    public void QuitGame()
    {
        Debug.Log("Fermeture du jeu !");
        Application.Quit(); 
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}