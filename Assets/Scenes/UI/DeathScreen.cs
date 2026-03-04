using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DeathManager : MonoBehaviour
{
    [Header("Réglages UI")]
    public GameObject deathPanel; // C'est ici que tu glisseras ton DeathMenuPanel !

    [Header("Affichage des Scores")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public GameObject newRecordAlert;

    void OnEnable() { SceneManager.sceneLoaded += OnLevelFinishedLoading; }
    void OnDisable() { SceneManager.sceneLoaded -= OnLevelFinishedLoading; }

    public void TriggerDeath()
    {
        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (ScoreManager.instance != null)
            {
                PlayerTimeManager timeManager = Object.FindFirstObjectByType<PlayerTimeManager>();
                float timeLeft = (timeManager != null) ? timeManager.currentTime : 0f;

                bool isNewRecord;
                int finalScore = ScoreManager.instance.CalculateFinalScore(timeLeft, out isNewRecord);
                int currentHighScore = ScoreManager.instance.GetHighScore();

                if (scoreText != null) scoreText.text = "Score : " + finalScore;
                if (highScoreText != null) highScoreText.text = "High Score : " + currentHighScore;

                if (newRecordAlert != null) newRecordAlert.SetActive(isNewRecord);
            }
        }
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        // On recherche le grand Canvas dans la nouvelle scène
        GameObject canvas = GameObject.Find("GameCanvas"); 
        
        if (canvas != null)
        {
            // CORRECTION ICI : on utilise "deathPanel", le vrai nom de ta variable !
            Transform foundPanel = canvas.transform.Find("DeathMenuPanel");
            
            if (foundPanel != null)
            {
                deathPanel = foundPanel.gameObject;
            }
        }
    }
}