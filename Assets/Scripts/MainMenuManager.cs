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
        // 1. ON LIBÈRE LA SOURIS (Pour pouvoir cliquer sur les boutons sans attaquer)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        // 2. ON NETHOYAGE LE JOUEUR EN ARRIÈRE-PLAN
        NeutralizePlayer();
    }

    private void NeutralizePlayer()
    {
        // On cherche le PlayerTimeManager qu'on vient de créer
        PlayerTimeManager ptm = Object.FindFirstObjectByType<PlayerTimeManager>();
        if (ptm != null)
        {
            ptm.gameStarted = false; // Bloque le chrono et les fonctions de tir
        }

        // On cherche le contrôleur de mouvement pour éviter qu'il tourne la tête ou saute
        ThirdPersonController controller = Object.FindFirstObjectByType<ThirdPersonController>();
        if (controller != null)
        {
            // On désactive le script pour qu'il ne lise plus les clics de souris
            controller.enabled = false; 
        }
    }

    // --- FONCTIONS DE JEU ---
    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void PlayTutorial()
    {
        SceneManager.LoadScene(tutorialSceneName);
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