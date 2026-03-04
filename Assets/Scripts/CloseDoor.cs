// using UnityEngine;
// using UnityEngine.AI; // NOUVEAU : On importe l'intelligence artificielle !
// namespace StarterAssets
// {// Ajoute automatiquement l'agent s'il manque
// public class CloseDoor : MonoBehaviour
// {
//     [Header("Cible & IA")]
//     private GameObject targetPlayer;
//     public bool isClosed = false;
//     private GoalEntry script;
//     void Start(){
//         targetPlayer = GameObject.Find("GoalEntry");
//         script = targetPlayer.GetComponent<GoalEntry>();
//     }
//     private void Update()
//     {
//         if (script.enabled){
//             this.gameObject.SetActive(true);
//             isClosed = true;
//         }
//     }

// }
// }

using UnityEngine;
using Cinemachine; // Pour reconnecter la caméra
using StarterAssets; // Pour trouver le ThirdPersonController

public class GoalEntry : MonoBehaviour
{
    [Header("Mise en scène")]
    [Tooltip("Glisse ici le faux joueur (l'acteur) qui marche tout seul")]
    public GameObject cinematicActor; 

    private bool hasSwapped = false;

    // Fonction appelée quand un objet rentre dans la zone GoalEntry
    private void OnTriggerEnter(Collider other)
    {
        // On vérifie si c'est bien l'acteur de la cinématique qui a atteint le but
        if (!hasSwapped && other.gameObject == cinematicActor)
        {
            SwapPlayers();
        }
    }

    // Le fameux tour de magie
    public void SwapPlayers()
    {
        hasSwapped = true;

        // 1. On trouve le vrai joueur caché (FindObjectsInactive est obligatoire car il est éteint)
        ThirdPersonController realPlayerScript = Object.FindFirstObjectByType<ThirdPersonController>(FindObjectsInactive.Include);

        if (realPlayerScript != null)
        {
            GameObject realPlayer = realPlayerScript.gameObject;

            // 2. On désactive sa physique temporairement pour le téléporter sans bug
            CharacterController cc = realPlayer.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            // 3. On téléporte le vrai joueur EXACTEMENT à la place de l'acteur
            realPlayer.transform.position = cinematicActor.transform.position;
            realPlayer.transform.rotation = cinematicActor.transform.rotation;

            // 4. On réactive la physique et on ALLUME le vrai joueur
            if (cc != null) cc.enabled = true;
            realPlayer.SetActive(true);

            // 5. On dit à la caméra de lâcher l'acteur et de filmer le vrai joueur
            CinemachineVirtualCamera vcam = Object.FindFirstObjectByType<CinemachineVirtualCamera>();
            if (vcam != null)
            {
                Transform cameraTarget = realPlayer.transform.Find("PlayerCameraRoot");
                if (cameraTarget == null) cameraTarget = realPlayer.transform;

                vcam.Follow = cameraTarget;
                vcam.LookAt = cameraTarget;
            }

            // 6. On détruit l'acteur (le faux joueur disparaît)
            Destroy(cinematicActor);

            // 7. On s'assure que le script est activé pour que ta porte (CloseDoor.cs) se ferme !
            this.enabled = true; 
            
            Debug.Log("Swap terminé ! Le vrai joueur reprend le contrôle avec ses armes et son chrono.");
        }
    }
}