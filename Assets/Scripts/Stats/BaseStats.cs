using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour, IJsonSaveable
    {
        //Used for setting level of NPCs in the inspector
        [Range(1,99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass_SO characterClass = null;
        [SerializeField] GameObject levelUpParticleEffect = null;
        [SerializeField] bool shouldUseModifiers = false;
        Experience experience;

        int currentLevel = 0;

        public event Action onLevelUp;

        private void Awake() {
            experience = GetComponent<Experience>();
        }

        private void Start() {
            if(currentLevel < startingLevel)
            {
                currentLevel = startingLevel;
            }
        }

        private void OnEnable() {
            if(experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable() {
            if(experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        }


        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel)
            {
                experience.ResetAfterLevelUp(GetCharacterStat(CharacterStat.ExperienceToLevelUp));
                currentLevel = newLevel;
                LevelUpEffect();
                onLevelUp();
                UpdateLevel(); //recursion in case of getting enough xp for multiple levelups at once
            }
        }

        private int CalculateLevel()
        {
            //update currentLevel to startingLevel for NPCs (no xp component)
            if(experience == null) return startingLevel;

            float currentXP = experience.GetExperienceTotal();
            float maxLevel = characterClass.GetLevels(CharacterStat.ExperienceToLevelUp);
            float ExperienceToLevelUp = GetCharacterStat(CharacterStat.ExperienceToLevelUp);
            if (currentLevel < maxLevel)
            {
                if(currentXP >= ExperienceToLevelUp)
                {
                    return currentLevel + 1;
                }
            }
            return currentLevel;
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect, transform);
        }

        public float GetCharacterStat(CharacterStat stat)
        {
            return GetBaseStat(stat) + GetAdditiveModifier(stat) * (1 + GetPercentageModifier(stat)/100);
        }

        private float GetBaseStat(CharacterStat stat)
        {
            return characterClass.GetCharacterStat(stat, currentLevel);
        }

        private float GetAdditiveModifier(CharacterStat stat)
        {
            if (!shouldUseModifiers) return 0;
            float total = 0;
            foreach(IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach(float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        private float GetPercentageModifier(CharacterStat stat)
        {
            if (!shouldUseModifiers) return 0;
            float total = 0;
            foreach(IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach(float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        public int GetLevel()
        {
            return currentLevel;
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(currentLevel);
        }

        public void RestoreFromJToken(JToken state)
        {
            currentLevel = state.ToObject<int>();
        }
    }
}