using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class RebindButton : MonoBehaviour
{
    [Header("Liaison avec le Joueur")]
    public PlayerInput playerInput; // Le composant du joueur qui contient le "clone" des touches

    [Header("Quelle action modifier ?")]
    public string actionName = "Move"; // Le nom de l'action (ex: "Move", "Jump", "Interact")
    public int bindingIndex = 2; // 2 = Avancer, 3 = Reculer, 4 = Gauche, 5 = Droite

    [Header("UI")]
    public TextMeshProUGUI buttonText;

    private InputAction actionToRebind;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    void Start()
    {
        // 1. Si on a oublié de lier le joueur, le script le cherche tout seul !
        if (playerInput == null)
        {
            Object.FindFirstObjectByType<PlayerInput>();
        }

        // 2. On récupère l'action directement depuis le cerveau du joueur (le clone)
        actionToRebind = playerInput.actions.FindAction(actionName);

        // 3. Charger les touches sauvegardées
        string savedRebinds = PlayerPrefs.GetString("rebinds", string.Empty);
        if (!string.IsNullOrEmpty(savedRebinds))
        {
            playerInput.actions.LoadBindingOverridesFromJson(savedRebinds);
        }
        
        UpdateUI();
    }

    public void StartRebinding()
    {
        buttonText.text = "...";
        
        actionToRebind.Disable();

        rebindingOperation = actionToRebind.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("<Mouse>/position")
            .WithControlsExcluding("<Mouse>/delta")
            .OnComplete(operation => RebindComplete())
            .Start();
    }

    private void RebindComplete()
    {
        UpdateUI();
        rebindingOperation.Dispose();

        actionToRebind.Enable();

        // On sauvegarde pour la prochaine fois
        string rebinds = playerInput.actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }

private void UpdateUI()
    {
        // 1. On récupère le nom brut donné par Unity (ex: "W", "E", "Space")
        string keyName = InputControlPath.ToHumanReadableString(
            actionToRebind.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);

        // 2. LA TRICHE ESTHÉTIQUE : Si Unity dit "Space", on le force à dire "_"
        if (keyName == "Space")
        {
            keyName = "_";
        }
        
        // (Bonus : Tu peux ajouter d'autres règles ici plus tard si besoin !)
        // if (keyName == "Left Button") keyName = "Clic";

        // 3. On applique le texte final sur le bouton
        buttonText.text = keyName;
    }
}