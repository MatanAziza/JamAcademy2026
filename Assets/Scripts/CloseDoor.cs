using UnityEngine;
using UnityEngine.AI; // NOUVEAU : On importe l'intelligence artificielle !
namespace StarterAssets
{// Ajoute automatiquement l'agent s'il manque
public class CloseDoor : MonoBehaviour
{
    [Header("Cible & IA")]
    private GameObject targetPlayer;
    public bool isClosed = false;
    private GoalEntry script;
    void Start(){
        targetPlayer = GameObject.Find("GoalEntry");
        script = targetPlayer.GetComponent<GoalEntry>();
    }
    private void Update()
    {
        if (script.enabled){
            this.gameObject.SetActive(true);
            isClosed = true;
        }
    }

}
}