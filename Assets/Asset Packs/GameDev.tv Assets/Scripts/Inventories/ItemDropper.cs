using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RPG.Saving;
using Newtonsoft.Json.Linq;

namespace GameDevTV.Inventories
{
    /// <summary>
    /// To be placed on anything that wishes to drop pickups into the world.
    /// Tracks the drops for saving and restoring.
    /// </summary>
    public class ItemDropper : MonoBehaviour, IJsonSaveable
    {
        // STATE
        private List<Pickup> droppedItems = new List<Pickup>();

        // PUBLIC

        /// <summary>
        /// Create a pickup at the current position.
        /// </summary>
        /// <param name="item">The item type for the pickup.</param>
        /// <param name="number">
        /// The number of items contained in the pickup. Only used if the item
        /// is stackable.
        /// </param>
        public void DropItem(InventoryItem item, int number)
        {
            SpawnPickup(item, GetDropLocation(), number);
        }

        /// <summary>
        /// Create a pickup at the current position.
        /// </summary>
        /// <param name="item">The item type for the pickup.</param>
        public void DropItem(InventoryItem item)
        {
            SpawnPickup(item, GetDropLocation(), 1);
        }

        // PROTECTED

        /// <summary>
        /// Override to set a custom method for locating a drop.
        /// </summary>
        /// <returns>The location the drop should be spawned.</returns>
        protected virtual Vector3 GetDropLocation()
        {
            return transform.position;
        }

        // PRIVATE

        public void SpawnPickup(InventoryItem item, Vector3 spawnLocation, int number)
        {
            var pickup = item.SpawnPickup(spawnLocation, number);
            droppedItems.Add(pickup);
        }

        [System.Serializable]
        private struct DropRecord
        {
            public string itemID;
            public SerializableVector3 position;
            public int number;
        }
 class otherSceneDropRecord
        {
            public string id;
            public int number;
            public Vector3 location;
            public int scene;
        }

        private List<otherSceneDropRecord> otherSceneDrops = new List<otherSceneDropRecord>();

        List<otherSceneDropRecord> MergeDroppedItemsWithOtherSceneDrops()
        {
            List<otherSceneDropRecord> result = new List<otherSceneDropRecord>();
            result.AddRange(otherSceneDrops);
            foreach (var item in droppedItems)
            {
                otherSceneDropRecord drop = new otherSceneDropRecord();
                drop.id = item.GetItem().GetItemID();
                drop.number = item.GetNumber();
                drop.location = item.transform.position;
                drop.scene = SceneManager.GetActiveScene().buildIndex;
                result.Add(drop);
            }
            return result;
        }
        
        public JToken CaptureAsJToken()
        {
            RemoveDestroyedDrops();
            var drops = MergeDroppedItemsWithOtherSceneDrops();
            JArray state = new JArray();
            IList<JToken> stateList = state;
            foreach (var drop in drops)
            {
                JObject dropState = new JObject();
                IDictionary<string, JToken> dropStateDict = dropState;
                dropStateDict["id"] = JToken.FromObject(drop.id);
                dropStateDict["number"] = drop.number;
                dropStateDict["location"] = drop.location.ToToken();
                dropStateDict["scene"] = drop.scene;
                stateList.Add(dropState);
            }

            return state;
        }

        private void ClearExistingDrops()
        {
            foreach (var oldDrop in droppedItems)
            {
                if (oldDrop != null) Destroy(oldDrop.gameObject);
            }

            otherSceneDrops.Clear();
        }
        
        public void RestoreFromJToken(JToken state)
        {
            if (state is JArray stateArray)
            {
                int currentScene = SceneManager.GetActiveScene().buildIndex;
                IList<JToken> stateList = stateArray;
                ClearExistingDrops();
                foreach (var entry in stateList)
                {
                    if (entry is JObject dropState)
                    {
                        IDictionary<string, JToken> dropStateDict = dropState;
                        int scene = dropStateDict["scene"].ToObject<int>();
                        InventoryItem item = InventoryItem.GetFromID(dropStateDict["id"].ToObject<string>());
                        int number = dropStateDict["number"].ToObject<int>();
                        Vector3 location = dropStateDict["location"].ToVector3();
                        if (scene == currentScene)
                        {
                            SpawnPickup(item, location, number);
                        }
                        else
                        {
                            var otherDrop = new otherSceneDropRecord();
                            otherDrop.id = item.GetItemID();
                            otherDrop.number = number;
                            otherDrop.location = location;
                            otherDrop.scene = scene;
                            otherSceneDrops.Add(otherDrop);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Remove any drops in the world that have subsequently been picked up.
        /// </summary>
        private void RemoveDestroyedDrops()
        {
            var newList = new List<Pickup>();
            foreach (var item in droppedItems)
            {
                if (item != null)
                {
                    newList.Add(item);
                }
            }
            droppedItems = newList;
        }
    }
}