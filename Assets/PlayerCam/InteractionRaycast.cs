using UnityEngine;
using UnityEngine.InputSystem; // OBLIGATOIRE pour lire les touches
using TMPro;

public class InteractionRaycast : MonoBehaviour
{
    [Header("Raycasting settings")]
    public float interactRange = 2f;
    public float heightOffset = 1f;

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
        Vector3 origin = transform.position + Vector3.up * heightOffset;
        Vector3 direction = transform.forward;

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
}