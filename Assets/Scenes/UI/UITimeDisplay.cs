using UnityEngine;
using TMPro; // NOUVEAU : Indispensable pour parler au texte TextMeshPro !

public class UITimeDisplay : MonoBehaviour
{
    [Header("Liaisons")]
    public PlayerTimeManager playerTime; // Le cerveau du joueur
    public TextMeshProUGUI timeText;     // Le composant texte à l'écran

    void Update()
    {
        // On vérifie que les deux cases sont bien remplies dans l'Inspector
        if (playerTime != null && timeText != null)
        {
            // Le "F1" permet d'afficher 1 seul chiffre après la virgule (ex: 59.9 s)
            // C'est beaucoup plus stressant et dynamique que des secondes entières !
            timeText.text = ""+ playerTime.currentTime.ToString("F1") + " s";
            
            // Bonus : Le texte devient rouge s'il reste moins de 10 secondes !
            if (playerTime.currentTime <= 10f)
            {
                timeText.color = Color.red;
            }
        }
    }
}