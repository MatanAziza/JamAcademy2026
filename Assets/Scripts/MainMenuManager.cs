using UnityEngine;
using UnityEngine.SceneManagement; 
using StarterAssets; // On en a besoin pour toucher au contrôleur du joueur

public class MainMenuManager : MonoBehaviour
{
    [Header("Configuration des Scènes")]
    public string gameSceneName = "Level_01"; 
    public string tutorialSceneName = "Tutorial_Level";

    [Header("Panneaux UI")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    private void Start()
    {
        // 1. ON LIBÈRE LA SOURIS
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        // 2. ON NEUTRALISE LE JOUEUR EN ARRIÈRE-PLAN
        NeutralizePlayer();
    }

    private void NeutralizePlayer()
    {
        PlayerTimeManager ptm = Object.FindFirstObjectByType<PlayerTimeManager>();
        if (ptm != null)
        {
            ptm.gameStarted = false; 
        }

        ThirdPersonController controller = Object.FindFirstObjectByType<ThirdPersonController>();
        if (controller != null)
        {
            controller.enabled = false; 
        }
    }

    // --- FONCTIONS DE JEU ---
    public void PlayGame()
    {
        Time.timeScale = 1f; // Sécurité
        SceneManager.LoadScene(gameSceneName);
    }

    public void PlayTutorial()
    {
        Time.timeScale = 1f; // Sécurité
        SceneManager.LoadScene(tutorialSceneName);
    }

    // --- NOUVELLE FONCTION : CHOIX DU NIVEAU DIRECT ---
    // Utilise ça sur tes nouveaux boutons de menu !
    public void LoadLevel(string levelName)
    {
        Time.timeScale = 1f; // Sécurité cruciale
        SceneManager.LoadScene(levelName);
    }

    // --- FONCTIONS DES PARAMÈTRES ---
    public void OpenSettings()
    {
        if(mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if(settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if(settingsPanel != null) settingsPanel.SetActive(false);
        if(mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit(); 
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}