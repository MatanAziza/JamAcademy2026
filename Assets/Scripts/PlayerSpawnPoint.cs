using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    void Start()
    {
        // On cherche le VRAI Joueur qui vient d'arriver de la Scène 1
        GameObject realPlayer = GameObject.FindGameObjectWithTag("Player");

        if (realPlayer != null)
        {
            // LA LIGNE MAGIQUE : On le met "au frigo" instantanément !
            realPlayer.SetActive(false);
            
            // (Optionnel mais recommandé) On le place au SpawnPoint pour éviter qu'il tombe dans le vide pendant qu'il est éteint
            realPlayer.transform.position = transform.position;
            realPlayer.transform.rotation = transform.rotation;
            
            Debug.Log("Le Vrai Joueur est caché. La cinématique peut commencer !");
        }
    }
}