using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Stats;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = ("RPG/Inventory/Drop Table"))]
    public class DropTable : ScriptableObject
    {
        //arrays to allow for different levels
        [SerializeField] float[] dropChancePercentage;
        [SerializeField] float[] minDrops;
        [SerializeField] float[] maxDrops;

        [SerializeField]
        DropConfig[] potentialDrops;

        [System.Serializable]
        //Specific item drop and numbers
        class DropConfig
        {
            public InventoryItem item;
            public float[] relativeChance;
            public int[] minNumber;
            public int[] maxNumber;
            public int GetRandomNumber(int level)
            {
                if (!item.IsStackable())
                {
                    return 1;
                }
                int min = GetByLevel(minNumber,level);
                int max = GetByLevel(maxNumber,level);
                return UnityEngine.Random.Range(min,max+1);
            }
        }

        public struct Dropped
        {
            public InventoryItem item;
            public int number;
        }

        public IEnumerable<Dropped> GetRandomDrops(int level)
        {
            if (!ShouldRandomDrop(level))
            {
                yield break;
            }
            for (int i = 0; i < GetRandomNumberOfDrops(level); i++)
            {
                yield return GetRandomDrop(level);
            }
        }

        bool ShouldRandomDrop(int level)
        {
            return UnityEngine.Random.Range(0,100) < GetByLevel(dropChancePercentage,level);
        }

        private int GetRandomNumberOfDrops(int level)
        {
            int min = (int) GetByLevel(minDrops,level);
            int max = (int) GetByLevel(maxDrops,level);
            return UnityEngine.Random.Range(min,max);
        }

        Dropped GetRandomDrop(int level)
        {
            Dropped randomDrop = new Dropped();
            DropConfig drop = SelectRandomItem(level);
            randomDrop.item = drop.item;
            randomDrop.number = drop.GetRandomNumber(level);

            return randomDrop;
        }

        DropConfig SelectRandomItem(int level)
        {
            float totalChance = GetTotalChance(level);
            float randomRoll = UnityEngine.Random.Range(0,totalChance);
            float chanceTotal = 0;
            foreach (var drop in potentialDrops)
            {
                chanceTotal += GetByLevel(drop.relativeChance,level);
                if (chanceTotal > randomRoll)
                {
                    return drop;
                }
            }
            return null;
        }

        private float GetTotalChance(int level)
        {
            float total = 0;
            foreach (var drop in potentialDrops)
            {
                total += GetByLevel(drop.relativeChance,level);
            }
            return total;
        }

        static T GetByLevel<T>(T[] values, int level)
        {
            if (values.Length == 0)
            {
                return default;
            }
            if (level > values.Length)
            {
                return values[values.Length-1];
            }
            if (level <= 0)
            {
                return default;
            }
            return values[level-1];
        }
    }
}