using UnityEngine;
using TMPro; // N'oublie pas cette ligne !

public class DeathManager : MonoBehaviour
{
    [Header("Réglages UI")]
    public GameObject deathPanel; // C'est ici que tu glisseras ton DeathMenuPanel !

    [Header("Affichage des Scores")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public GameObject newRecordAlert;

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
}