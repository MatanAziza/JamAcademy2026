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
        // Mise en place du Singleton
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    // --- FONCTIONS POUR COMPTER ---

    public void AddEnemyKill()
    {
        enemiesDefeated++;
        // Debug.Log("Ennemi tué ! Total : " + enemiesDefeated);
    }

    public void AddHitReceived()
    {
        hitsReceived++;
        // Debug.Log("Coup reçu ! Total : " + hitsReceived);
    }

    // --- CALCUL FINAL (À appeler à la fin du niveau) ---

// On ajoute "out bool isNewHighScore" pour que la fonction nous dise si on a battu le record
    public int CalculateFinalScore(float remainingTime, out bool isNewHighScore)
    {
        int timeScore = Mathf.RoundToInt(remainingTime) * pointsPerSecond;
        int enemyScore = enemiesDefeated * pointsPerEnemy;
        int hitPenalty = hitsReceived * penaltyPerHit;

        int finalScore = timeScore + enemyScore - hitPenalty;
        if (finalScore < 0) finalScore = 0; // Pas de score négatif

        // --- GESTION DU HIGH SCORE ---
        int currentHighScore = PlayerPrefs.GetInt("HighScore", 0); // Récupère l'ancien record (0 par défaut)
        
        if (finalScore > currentHighScore)
        {
            isNewHighScore = true;
            PlayerPrefs.SetInt("HighScore", finalScore); // On sauvegarde le nouveau record
            PlayerPrefs.Save();
        }
        else
        {
            isNewHighScore = false;
        }

        return finalScore;
    }

    // Petite fonction utilitaire pour lire le record n'importe quand
    public int GetHighScore()
    {
        return PlayerPrefs.GetInt("HighScore", 0);
    }
}