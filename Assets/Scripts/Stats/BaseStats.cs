using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1,99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass_SO characterClass = null;

        public float GetHealth()
        {
            return characterClass.GetHealth(startingLevel);
        }

        public float GetExperienceReward()
        {
            return 10;
        }
    }
}