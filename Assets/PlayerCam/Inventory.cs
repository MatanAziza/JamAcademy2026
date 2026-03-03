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
    private int currentSlot = 1;
    public float switchTime = 1f;
    public float switchCooldown = 3f;
    private float switchCdTimer;
    private bool canSwitch = true;
    private bool lastSwitch_state;

    private void Start()
    {
        ShowItem(currentSlot);
        _input = GetComponent<StarterAssetsInputs>();
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
        if (switchCdTimer != 0f){
            _input.switch_item = false;
        }
        if (_input.switch_item != lastSwitch_state && currentSlot == 2 && canSwitch)
        {
            currentSlot = 1;
            ShowItem(currentSlot);
        }

        else if (_input.switch_item != lastSwitch_state && currentSlot == 1 && canSwitch)
        {
            currentSlot = 2;
            ShowItem(currentSlot);
        }
        lastSwitch_state = _input.switch_item;
    }

    void ShowItem(int slot)
    {
        // Supprime l'ancien item affiché
        if (currentItem != null)
        {
            Destroy(currentItem);
        }

        GameObject itemToShow = null;

        if (slot == 1)
            itemToShow = item1;
        else if (slot == 2)
            itemToShow = item2;

        if (itemToShow != null)
        {
            currentItem = Instantiate(itemToShow, hand);
            Debug.Log("J'affiche l'item " + currentItem);
            currentItem.transform.localPosition = Vector3.zero;
            currentItem.transform.localRotation = Quaternion.identity;
        }
        canSwitch = false;
    }
}
}