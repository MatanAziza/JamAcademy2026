using UnityEngine;
using UnityEngine.SceneManagement; // Indispensable pour changer de scène

public class LevelChanger : MonoBehaviour
{
    [Header("Réglages")]
    public string nextSceneName; // Tape le nom exact de la scène de ton collègue
    public bool useIndexInstead = true; // Si coché, il prendra juste la scène suivante dans le Build Profile

    private bool isLoading = false;

    private void OnTriggerEnter(Collider other)
    {
        // On vérifie si c'est le joueur qui touche l'objet
        if (other.CompareTag("Player") && !isLoading)
        {
            isLoading = true;
            Debug.Log("Transition vers le niveau suivant...");

            if (useIndexInstead)
            {
                // Récupère l'index actuel et ajoute 1
                int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

                // Vérifie si la scène suivante existe bien dans le Build Profile
                if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
                {
                    SceneManager.LoadScene(nextSceneIndex);
                }
                else
                {
                    Debug.LogWarning("Pas de scène suivante dans le Build Profile !");
                    isLoading = false;
                }
            }
            else
            {
                // Charge par nom
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }
}