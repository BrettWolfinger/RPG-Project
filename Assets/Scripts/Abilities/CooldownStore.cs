using System;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;

namespace RPG.Abilities
{ 
    public class CooldownStore : MonoBehaviour 
    {
        Dictionary<InventoryItem, float> cooldownTimers = new Dictionary<InventoryItem, float>();
        Dictionary<InventoryItem, float> initialCooldownTimers = new Dictionary<InventoryItem, float>();

        void Update() {
            List<InventoryItem> keys = new List<InventoryItem>(cooldownTimers.Keys);
            foreach (InventoryItem inventoryItem in keys)
            {
                cooldownTimers[inventoryItem] -= Time.deltaTime;
                if(cooldownTimers[inventoryItem] < 0)
                {
                    cooldownTimers.Remove(inventoryItem);
                    initialCooldownTimers.Remove(inventoryItem);
                }
            }
        }

        public void StartCooldown(InventoryItem inventoryItem, float cooldownTime)
        {
            cooldownTimers[inventoryItem] = cooldownTime;
            initialCooldownTimers[inventoryItem] = cooldownTime;
        }

        public float GetTimeRemaining(InventoryItem inventoryItem)
        {
            if(!cooldownTimers.ContainsKey(inventoryItem))
            {
                return 0;
            }

            return cooldownTimers[inventoryItem];
        }

        public float GetFractionRemaining(InventoryItem inventoryItem)
        {
            if (inventoryItem == null)
            {
                return 0;
            }

            if(!cooldownTimers.ContainsKey(inventoryItem))
            {
                return 0;    
            }

            return cooldownTimers[inventoryItem] / initialCooldownTimers[inventoryItem];
        }
    }
}