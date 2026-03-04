using UnityEngine;
using Cinemachine;
using StarterAssets; // IMPORTANT : Pour réactiver le mouvement

public class SafeSwap : MonoBehaviour
{
    [Header("Réglages Caméra")]
    public int vcamPriority = 100;

    private void OnTriggerEnter(Collider other)
    {
        // 1. On détecte l'ACTEUR (Untagged + CharacterController)
        if (other.CompareTag("Untagged") && other.GetComponent<CharacterController>() != null)
        {
            GameObject actor = other.gameObject;
            GameObject realPlayer = null;

            // 2. RECHERCHE DU VRAI JOUEUR (Tag "Player")
            // On cherche l'objet qui a survécu au Main Menu
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            
            foreach (GameObject p in players)
            {
                // On s'assure que c'est une instance dans la scène et pas l'acteur lui-même
                if (p != actor && p.scene.name != null)
                {
                    realPlayer = p;
                    break;
                }
            }

            // Secours si le joueur est désactivé (FindGameObjectsWithTag ne le verra pas)
            if (realPlayer == null)
            {
                var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
                foreach (var go in allObjects)
                {
                    if (go.CompareTag("Player") && go != actor && go.scene.name != null)
                    {
                        realPlayer = go;
                        break;
                    }
                }
            }

            if (realPlayer != null)
            {
                // 3. TÉLÉPORTATION ET PHYSIQUE
                CharacterController playerCC = realPlayer.GetComponent<CharacterController>();
                
                // On coupe la physique pour le TP
                if (playerCC != null) playerCC.enabled = false;
                
                realPlayer.transform.SetParent(null); // On le libère de tout parent
                realPlayer.transform.position = actor.transform.position;
                realPlayer.transform.rotation = actor.transform.rotation;

                // 4. RÉACTIVATION DU JOUEUR ET DES SCRIPTS
                realPlayer.SetActive(true);

                // Réveil du mouvement (Starter Assets)
                ThirdPersonController controller = realPlayer.GetComponent<ThirdPersonController>();
                if (controller != null) 
                {
                    controller.enabled = true;
                    controller.gameObject.SendMessage("OnReset", SendMessageOptions.DontRequireReceiver);
                }
                // Réveil du Timer
                PlayerTimeManager timeManager = realPlayer.GetComponent<PlayerTimeManager>();
                if (timeManager != null) timeManager.gameStarted = true;

                // Rallumer la physique
                if (playerCC != null) playerCC.enabled = true;

                // 5. VERROUILLAGE DE LA SOURIS (Indispensable après le menu)
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                // 6. FIX CAMÉRA (Cinemachine)
                CinemachineVirtualCamera vcam = Object.FindFirstObjectByType<CinemachineVirtualCamera>();
                if (vcam != null)
                {
                    Transform camTarget = realPlayer.transform.Find("PlayerCameraRoot");
                    vcam.Follow = (camTarget != null) ? camTarget : realPlayer.transform;
                    vcam.LookAt = (camTarget != null) ? camTarget : realPlayer.transform;
                    vcam.Priority = vcamPriority;
                }

                // 7. RECONEXION DU RAYCAST D'INTERACTION
                InteractionRaycast raycast = realPlayer.GetComponent<InteractionRaycast>();
                if (raycast != null) raycast.FindReferences();

                // 8. NETTOYAGE
                Debug.Log("SWAP : " + realPlayer.name + " est réveillé et prêt !");
                Destroy(actor);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("SWAP IMPOSSIBLE : Le vrai joueur est introuvable !");
            }
        }
    }
}