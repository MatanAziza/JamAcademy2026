using UnityEngine;
using UnityEngine.UI;

public class InteractionRaycast : MonoBehaviour
{
    [Header("Réglages")]
    public float distance = 3f;
    public LayerMask layerMask;
    
    [Header("UI")]
    public GameObject interactionUI; // Ton texte "Appuyer sur E"

    private Camera _mainCam;

    void Start()
    {
        FindReferences();
    }

    // Fonction pour re-lier les objets après un changement de scène ou un swap
    public void FindReferences()
    {
        _mainCam = Camera.main;
        
        // Si l'UI est perdue, on la recherche dans le nouveau GameCanvas
        if (interactionUI == null)
        {
            GameObject canvas = GameObject.Find("GameCanvas");
            if (canvas != null)
            {
                // Remplace "InteractionText" par le nom exact de ton UI dans la hiérarchie
                Transform t = canvas.transform.Find("InteractionText");
                if (t != null) interactionUI = t.gameObject;
            }
        }
    }

    void Update()
    {
        // Sécurité 1 : Si la caméra est perdue (cas du swap)
        if (_mainCam == null) 
        {
            _mainCam = Camera.main;
            if (_mainCam == null) return;
        }

        Ray ray = new Ray(_mainCam.transform.position, _mainCam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distance, layerMask))
        {
            // Sécurité 2 : On vérifie si l'objet a un collider et un script
            if (hit.collider != null)
            {
                // Ici, on cherche si l'objet est interactif
                // Remplace 'IInteractable' par ton script (ex: Door, Item)
                var target = hit.collider.gameObject;

                if (target != null)
                {
                    // On affiche l'UI seulement si elle est assignée
                    if (interactionUI != null) interactionUI.SetActive(true);

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        // Logique d'interaction ici
                        Debug.Log("Interaction avec : " + target.name);
                    }
                }
            }
        }
        else
        {
            // Sécurité 3 : On cache l'UI si on ne regarde rien
            if (interactionUI != null && interactionUI.activeSelf)
            {
                interactionUI.SetActive(false);
            }
        }
    }
}