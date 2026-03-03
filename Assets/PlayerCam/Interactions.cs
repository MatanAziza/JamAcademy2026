using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class Collectible3D : MonoBehaviour
    {
        public float rotationSpeed = 0.5f;

        public bool isInteracting = false;
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
                return;
            }

            StarterAssetsInputs playerInput = player.GetComponent<StarterAssetsInputs>();
            if (playerInput == null){
                return ;
            }

            // Collecte seulement si pas déjà collecté
            if (playerInput.interact)
            {
                isInteracting = true;
                Debug.Log("Item récupéré");
                Inventory inv = other.GetComponent<Inventory>();
                Collider trigger = GetComponent<Collider>();
                trigger.enabled = false;
                inv.item2 = this.gameObject;
                inv.item2.transform.localPosition = new Vector3(inv.xTranslation, inv.yTranslation, inv.zTranslation);
                rotationSpeed = 0f;
                isInteracting = false;
            }
        }
    }
}
