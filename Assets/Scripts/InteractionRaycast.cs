using UnityEngine;
using UnityEngine.InputSystem; // OBLIGATOIRE pour lire les touches
using TMPro;

public class InteractionRaycast : MonoBehaviour
{
    [Header("Raycasting settings (Interaction)")]
    public float interactRange = 2f;
    public float heightOffset = 1f;

    [Header("Raycasting settings (Tir / Ranged)")]
    public float shootRange = 15f; // La balle va beaucoup plus loin
    public LayerMask enemyLayers;  // Filtre pour que le tir ne touche QUE les ennemis

    [Header("Input Linking")]
    public PlayerInput playerInput; 
    public string interactActionName = "Interact";
    private string currentInteractKey = "E";

    [Header("UI Linking")]
    public TextMeshProUGUI interactionText;

    void Start()
    {
        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }
        UpdateKeyDisplay();
    }

    public void UpdateKeyDisplay()
    {
        if (playerInput != null)
        {
            InputAction interactAction = playerInput.actions.FindAction(interactActionName);
            
            // On vérifie que l'action existe ET qu'elle a au moins 1 touche d'assignée !
            if (interactAction != null && interactAction.bindings.Count > 0)
            {
                currentInteractKey = InputControlPath.ToHumanReadableString(
                    interactAction.bindings[0].effectivePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice);

                if (currentInteractKey == "Space") currentInteractKey = "_";
            }
            else
            {
                currentInteractKey = "?"; // Si aucune touche n'existe, on affiche un point d'interrogation
            }
        }
    }

    void Update()
    {
        // --- 1. LE HELPER (Tourne en boucle) ---
        Vector3 origin = transform.position + Vector3.up * heightOffset;
        Vector3 direction = transform.forward;

        // Le rayon rouge court pour l'interaction
        Debug.DrawRay(origin, direction * interactRange, Color.red);

        RaycastHit hit;

        if (Physics.Raycast(origin, direction, out hit, interactRange))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                interactionText.text = "Press [" + currentInteractKey + "] to interact";
                return; 
            }
        }

        interactionText.text = "";
    }

    // --- 2. LE TIR (Appelé uniquement quand on attaque) ---
    public void PerformRangedAttack(int damage, float timeReward)
    {
        Vector3 origin = transform.position + Vector3.up * heightOffset;
        Vector3 direction = transform.forward;

        // Un rayon bleu qui reste affiché 0.5s pour t'aider à visualiser tes tirs dans l'éditeur
        Debug.DrawRay(origin, direction * shootRange, Color.blue, 0.5f);

        RaycastHit hit;

        // Ici on utilise le LayerMask "enemyLayers" défini dans l'inspecteur.
        // Le rayon ignorera les murs basiques ou les interactables si on le règle bien.
        if (Physics.Raycast(origin, direction, out hit, shootRange, enemyLayers))
        {
            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, timeReward);
                Debug.Log("Tir réussi sur " + hit.collider.name + " !");
            }
        }
    }
}