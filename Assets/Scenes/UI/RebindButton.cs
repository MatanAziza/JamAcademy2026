using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class RebindButton : MonoBehaviour
{
    [Header("Liaison avec le Joueur")]
    public PlayerInput playerInput;

    [Header("Quelle action modifier ?")]
    public string actionName; 
    public int bindingIndex;  

    [Header("UI")]
    public TextMeshProUGUI buttonText;

    private InputAction actionToRebind;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    void Start()
    {
        if (playerInput == null)
        {
            playerInput = Object.FindFirstObjectByType<PlayerInput>();
        }

        actionToRebind = playerInput.actions.FindAction(actionName);

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
            .WithCancelingThrough("<Keyboard>/escape") 
            .OnComplete(operation => RebindComplete()) 
            .OnCancel(operation => RebindComplete())   
            .Start();
    }

    private void RebindComplete()
    {
        UpdateUI();
        rebindingOperation.Dispose();

        actionToRebind.Enable(); 

        string rebinds = playerInput.actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }

    // --- LA NOUVELLE FONCTION DE RESET ---
    public void ResetAllBindings()
    {
        // 1. On supprime la sauvegarde des touches personnalisées
        PlayerPrefs.DeleteKey("rebinds");

        // 2. On force le système à oublier toutes les modifications
        playerInput.actions.RemoveAllBindingOverrides();

        // 3. On cherche tous les boutons de rebinding à l'écran pour rafraîchir leur texte
        RebindButton[] allButtons = Object.FindObjectsByType<RebindButton>(FindObjectsSortMode.None);
        foreach (RebindButton btn in allButtons)
        {
            btn.UpdateUI();
        }
    }

    // Attention : UpdateUI est passé en "public" pour que le Reset puisse l'appeler
    public void UpdateUI()
    {
        if (actionToRebind == null) return;

        string rawKeyName = InputControlPath.ToHumanReadableString(
            actionToRebind.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);

        buttonText.text = FormatKeyName(rawKeyName);
    }

    public string FormatKeyName(string rawKeyName)
    {
        if (rawKeyName.Contains("Left Button") || rawKeyName == "Mouse0" || rawKeyName == "LMB")
            return "LEFT CLICK";
            
        if (rawKeyName.Contains("Right Button") || rawKeyName == "Mouse1" || rawKeyName == "RMB")
            return "RIGHT CLICK";
            
        if (rawKeyName.ToLower() == "space")
            return "SPACE";
            
        if (rawKeyName.ToLower() == "tab")
            return "TAB";

        return rawKeyName.ToUpper();
    }
}