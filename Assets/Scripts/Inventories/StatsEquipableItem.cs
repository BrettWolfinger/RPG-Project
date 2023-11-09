using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Stats;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = ("RPG/Inventory/Equipable Item"))]
    public class StatsEquipableItem : EquipableItem, IModifierProvider
    {
        [SerializeField]
        Modifier[] additiveModifiers;
        [SerializeField]
        Modifier[] percentageModifiers;

        [System.Serializable]
        struct Modifier
        {
            public CharacterStat stat;
            public float value;
        }

        public IEnumerable<float> GetAdditiveModifiers(CharacterStat stat)
        {
            foreach(Modifier additiveModifier in additiveModifiers)
            {
                if(additiveModifier.stat == stat)
                {
                    yield return additiveModifier.value;
                }
            }
        }

        public IEnumerable<float> GetPercentageModifiers(CharacterStat stat)
        {
            foreach(Modifier percentageModifier in percentageModifiers)
            {
                if(percentageModifier.stat == stat)
                {
                    yield return percentageModifier.value;
                }
            }
        }

    }
}
