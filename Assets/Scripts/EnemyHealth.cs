using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Statistiques")]
    public int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // On ajoute ", float timeReward" pour qu'il accepte le temps envoyé par le joueur
    public void TakeDamage(int damageAmount, float timeReward)
    {
        currentHealth -= damageAmount;
        Debug.Log(gameObject.name + " prend " + damageAmount + " dégâts !");

        // Si l'ennemi meurt, on fait passer la récompense de temps à la fonction Die
        if (currentHealth <= 0)
        {
            Die(timeReward);
        }
    }

    // La fonction Die reçoit le temps et détruit l'ennemi
    private void Die(float timeGained)
    {
        Debug.Log("Ennemi achevé ! Temps gagné : " + timeGained + " secondes.");
        PlayerTimeManager timeManager = Object.FindFirstObjectByType<PlayerTimeManager>();
        if (timeManager != null)
        {
            timeManager.AddTimeReward(timeGained);
        }
        ScoreManager.instance.AddEnemyKill();
        Destroy(gameObject);
    }
}