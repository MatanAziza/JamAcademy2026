using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    void Start()
    {
        // 1. On cherche le vrai joueur (celui qui a survécu avec le Tag "Player")
        GameObject realPlayer = GameObject.FindGameObjectWithTag("Player");

        if (realPlayer != null)
        {
            // 2. On le met "au frigo" (invisible et inactif) le temps de la cinématique
            // Comme ça, c'est le faux joueur (l'acteur) qu'on verra à l'écran.
            realPlayer.SetActive(false);
            
            Debug.Log("Vrai joueur caché. Place à la cinématique !");
        }
        else
        {
            Debug.LogWarning("Aucun vrai joueur trouvé par le SpawnPoint.");
        }
    }
}