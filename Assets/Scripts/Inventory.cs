using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif 
public class Inventory : MonoBehaviour
{
    public GameObject item1;
    public GameObject item2;

    public Transform hand;

    private GameObject currentItem;
    private StarterAssetsInputs _input;
    public int currentSlot = 1;
    public float switchTime = 1f;
    public float switchTimer = 0f;
    public float switchCooldown = 1f;
    public float switchBuffer = 0.1f;
    public float switchCdTimer;
    public bool canSwitch = true;
    public bool isSwitching = false;
    public bool lastSwitch_state;
    [Range(-0.5f, 0.5f)]
    public float xTranslation;
    [Range(-0.5f, 0.5f)]
    public float yTranslation;
    [Range(-0.5f, 0.5f)]
    public float zTranslation;
    private ThirdPersonController player;

    private void Awake()
    {
        DontDestroyOnLoad(transform.root.gameObject);
        // 2. Sécurité anti-doublon (très important pour les tests)
        // Si on arrive dans une scène qui a déjà un joueur placé à la main, 
        // on garde celui qui vient de la scène précédente et on détruit le nouveau.
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 1)
        {
            foreach (GameObject p in players)
            {
                if (p != this.gameObject)
                {
                    Destroy(p);
                }
            }
        }
    }
    
    private void Start()
    {
        ShowItem(currentSlot);
        _input = GetComponent<StarterAssetsInputs>();
		player = GetComponent<ThirdPersonController>();
    }

    void Update()
    {
        if (!canSwitch) {
                switchCdTimer += Time.deltaTime;
                if (switchCdTimer >= switchCooldown) {
                    canSwitch = true;
                    switchCdTimer = 0f;
                }
            }
        if (switchCdTimer != 0f && switchCdTimer < switchCooldown ){
            _input.switch_item = false;
        }
        if (_input.switch_item != lastSwitch_state && currentSlot == 2 && canSwitch && !player.isDashing && !player.isAttacking)
        {
            currentSlot = 1;
            ShowItem(currentSlot);
        }

        else if (_input.switch_item != lastSwitch_state && currentSlot == 1 && canSwitch)
        {
            currentSlot = 2;
            ShowItem(currentSlot);
        }
        if (isSwitching) {
                switchTimer += Time.deltaTime;

                if (switchTimer > switchTime)
                    isSwitching = false;
            }
        lastSwitch_state = _input.switch_item;
    }

    public void ShowItem(int slot)
    {
        // ... (leur code pour détruire l'ancien item) ...

        GameObject itemToShow = null;
        if (slot == 1) itemToShow = item1;
        else if (slot == 2) itemToShow = item2;

        if (itemToShow != null)
        {
            Destroy(currentItem);
            currentItem = Instantiate(itemToShow, hand);
            currentItem.transform.localPosition = new Vector3(xTranslation, yTranslation, zTranslation);
            
            // ---> TA LIGNE DE DEBUG ICI <---
            Debug.Log("ÉQUIPEMENT ACTUEL : Le slot " + slot + " contient l'arme [" + itemToShow.name + "]");
        }
        else
        {
            Debug.Log("ÉQUIPEMENT ACTUEL : Le slot " + slot + " est VIDE (Mains nues)");
        }
        
        canSwitch = false;
        isSwitching = true;
    }

    // public void ShowItem(int slot)
    // {
    //     // Supprime l'ancien item affiché
    //     if (currentItem != null)
    //     {
    //         Destroy(currentItem);
    //     }

    //     GameObject itemToShow = null;

    //     if (slot == 1)
    //         itemToShow = item1;
    //     else if (slot == 2)
    //         itemToShow = item2;

    //     if (itemToShow != null)
    //     {
    //         currentItem = Instantiate(itemToShow, hand);
    //         currentItem.transform.localPosition = new Vector3(xTranslation, yTranslation, zTranslation);
    //         Debug.Log("J'affiche l'item " + currentItem);
    //     }
    //     canSwitch = false;
    //     isSwitching = true;
    // }
}
}