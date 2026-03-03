using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StarterAssets; 

public class InventoryUIManager : MonoBehaviour
{
    [Header("HUD Armes (Bas Gauche)")]
    public Image slot1Icon; 
    public Image slot2Icon; 
    public RectTransform highlightBox; 
    public RectTransform slot1Transform; 
    public RectTransform slot2Transform; 

    [Header("Loot Window")]
    public GameObject lootWindowPanel;
    public TextMeshProUGUI lootText;

    // LE 3ÈME SLOT INVISIBLE (Buffer)
    private GameObject bufferWeapon; 
    private Spawner currentSpawner; 
    
    // La fameuse variable qui était en double :
    private Inventory playerInventory;

    void Start()
    {
        // On cherche l'inventaire du joueur au démarrage
        playerInventory = Object.FindFirstObjectByType<Inventory>();
    }

    void Update()
    {
        if (playerInventory != null)
        {
            UpdateHUDHighlight();
            // Décommente la ligne en dessous quand tes collègues auront ajouté WeaponInfo.cs sur leurs armes
            // UpdateHUDIcons(); 
        }
    }

    // --- PARTIE LOOT WINDOW ---

    public void OpenLootWindow(GameObject weaponPrefab, Spawner spawner, Inventory inv)
    {
        bufferWeapon = weaponPrefab;
        currentSpawner = spawner;
        playerInventory = inv;

        string cleanName = weaponPrefab.name.Replace("(Clone)", "");
        lootText.text = "Do you want to add " + cleanName + " to your inventory?";

        lootWindowPanel.SetActive(true);
        Time.timeScale = 0f; 
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void TakeItem()
    {
        if (playerInventory.currentSlot == 1)
        {
            playerInventory.item1 = bufferWeapon;
        }
        else
        {
            playerInventory.item2 = bufferWeapon;
        }

        playerInventory.ShowItem(playerInventory.currentSlot);

        if (currentSpawner != null) Destroy(currentSpawner.gameObject);

        CloseLootWindow();
    }

    public void LeaveItem()
    {
        if (currentSpawner != null)
        {
            currentSpawner.GetComponent<Collider>().enabled = true;
        }
        CloseLootWindow();
    }

    private void CloseLootWindow()
    {
        bufferWeapon = null;
        currentSpawner = null;
        lootWindowPanel.SetActive(false);
        Time.timeScale = 1f; 
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // --- PARTIE HUD BAS GAUCHE ---

    private void UpdateHUDHighlight()
    {
        if (playerInventory.currentSlot == 1)
        {
            highlightBox.position = slot1Transform.position;
        }
        else if (playerInventory.currentSlot == 2)
        {
            highlightBox.position = slot2Transform.position;
        }
    }

    private void UpdateHUDIcons()
    {
        if (playerInventory.item1 != null)
        {
            WeaponInfo info = playerInventory.item1.GetComponent<WeaponInfo>();
            if (info != null && info.weaponIcon != null)
            {
                slot1Icon.sprite = info.weaponIcon;
                slot1Icon.color = Color.white; 
            }
        }
        else
        {
            slot1Icon.color = new Color(1, 1, 1, 0); 
        }

        if (playerInventory.item2 != null)
        {
            WeaponInfo info = playerInventory.item2.GetComponent<WeaponInfo>();
            if (info != null && info.weaponIcon != null)
            {
                slot2Icon.sprite = info.weaponIcon;
                slot2Icon.color = Color.white;
            }
        }
        else
        {
            slot2Icon.color = new Color(1, 1, 1, 0); 
        }
    }
}