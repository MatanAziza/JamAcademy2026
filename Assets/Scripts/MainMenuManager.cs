using UnityEngine;
using UnityEngine.SceneManagement; // OBLIGATOIRE pour changer de scène

public class MainMenuManager : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Le nom exact de ta scène de jeu (attention aux majuscules)")]
    public string gameSceneName = "Level_01"; 

    // --- FONCTION JOUER ---
    public void PlayGame()
    {
        Debug.Log("Lancement du jeu...");
        // Recharge le temps normal au cas où on aurait quitté le jeu en pause (Time.timeScale = 0)
        Time.timeScale = 1f; 
        SceneManager.LoadScene(gameSceneName);
    }

    // --- FONCTION QUITTER ---
    public void QuitGame()
    {
        Debug.Log("Fermeture du jeu !");
        
        // Ferme le vrai jeu une fois compilé
        Application.Quit(); 

        // Ligne magique qui arrête le mode "Play" directement dans l'éditeur Unity pour tester
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}