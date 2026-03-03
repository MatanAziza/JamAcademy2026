using UnityEngine;
using TMPro; // Toujours indispensable pour le texte UI

public class UIWeaponDisplay : MonoBehaviour
{
    [Header("Liaison")]
    public TextMeshProUGUI weaponText;

    void Start()
    {
        // On affiche l'arme de base au lancement du jeu
        UpdateWeapon("Melee", "Cost : 0s"); 
    }

    // C'est CETTE fonction que ton coéquipier va utiliser !
    public void UpdateWeapon(string weaponName, string ammoInfo)
    {
        if (weaponText != null)
        {
            // Le \n permet de faire un retour à la ligne
            weaponText.text = weaponName + "\n" + ammoInfo;
        }
    }
}