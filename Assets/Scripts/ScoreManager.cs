using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // Le Singleton permet à n'importe quel script de parler au ScoreManager facilement
    public static ScoreManager instance;

    [Header("Statistiques de la partie (Lecture Seule)")]
    public int enemiesDefeated = 0;
    public int hitsReceived = 0;

    [Header("Équilibrage des Points")]
    public int pointsPerEnemy = 100;     // Bonus par ennemi
    public int pointsPerSecond = 10;     // Bonus par seconde restante
    public int penaltyPerHit = 50;       // Malus par coup reçu

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddEnemyKill()
    {
        enemiesDefeated++;
    }

    public void AddHitReceived()
    {
        hitsReceived++;
    }

    public int CalculateFinalScore(float remainingTime, out bool isNewHighScore)
    {
        int timeScore = Mathf.RoundToInt(remainingTime) * pointsPerSecond;
        int enemyScore = enemiesDefeated * pointsPerEnemy;
        int hitPenalty = hitsReceived * penaltyPerHit;

        int finalScore = timeScore + enemyScore - hitPenalty;
        if (finalScore < 0) finalScore = 0;

        int currentHighScore = PlayerPrefs.GetInt("HighScore", 0);
        
        if (finalScore > currentHighScore)
        {
            isNewHighScore = true;
            PlayerPrefs.SetInt("HighScore", finalScore);
            PlayerPrefs.Save();
        }
        else
        {
            isNewHighScore = false;
        }

        return finalScore;
    }

    public int GetHighScore()
    {
        return PlayerPrefs.GetInt("HighScore", 0);
    }
}