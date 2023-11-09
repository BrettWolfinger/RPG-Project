using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.UI;
using UnityEngine;
using UnityEngine.EventSystems;
    
    namespace RPG.Control
    {
        [RequireComponent(typeof(Pickup))]
        public class ClickablePickup : MonoBehaviour, IRaycastable
        {
            Pickup pickup;
     
            private void Awake()
            {
                pickup = GetComponent<Pickup>();
            }
     
            public bool HandleRaycast(PlayerController callingController)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    pickup.PickupItem();
                }
                return true;
            }

            public CursorType_SO GetCursorType(PlayerController callingController)
            {
                    return callingController.cursors.PickupCursor;
            }
        }
    }