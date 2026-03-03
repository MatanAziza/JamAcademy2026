using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class Spawner : MonoBehaviour
    {
        public float rotationSpeed = 0.5f;
        public GameObject[] weapons;
        public bool isInteracting = false;
        private bool interactTimer;
        public bool next_room = false;
        public bool pick_item = false;
        public bool dialog = false;
        

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }

        void OnTriggerStay(Collider other)
        {
            // Vérifie que l'autre est bien le joueur
            ThirdPersonController player = other.GetComponent<ThirdPersonController>();
            if (player == null) {
                Debug.Log("haha");
                return;
            }

            StarterAssetsInputs playerInput = player.GetComponent<StarterAssetsInputs>();
            if (playerInput == null){
                Debug.Log("hahahahahaha");
                return ;
            }

            // Collecte seulement si pas déjà collecté
            if (playerInput.interact && GetComponent<Collider>().enabled)
            {
                isInteracting = true;
                GameObject gettableWeapon = weapons[Random.Range(0, weapons.Length)];
                Debug.Log("Item récupéré");
                Inventory inv = other.GetComponent<Inventory>();
                //-----------------------------------------
                Collider trigger = GetComponent<Collider>();
                trigger.enabled = false;
                inv.item2 = gettableWeapon;
                inv.item2.transform.localPosition = new Vector3(inv.xTranslation, inv.yTranslation, inv.zTranslation);
                inv.ShowItem(2);
                inv.currentSlot = 2;
                Destroy(this.gameObject);
                //-----------------------------------------
                InventoryUIManager uiManager = Object.FindFirstObjectByType<InventoryUIManager>();
                if (uiManager != null)
                {
                    // On désactive le collider pour éviter que le joueur spamme la touche
                    GetComponent<Collider>().enabled = false;
                    
                    // On envoie l'arme tirée au hasard dans un "3ème slot" (buffer)
                    uiManager.OpenLootWindow(gettableWeapon, this, inv);
                }
                rotationSpeed = 0f;
                isInteracting = false;
            }
        }
    }
}
