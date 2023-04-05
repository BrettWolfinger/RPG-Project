using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;
using System;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, IJsonSaveable 
    {
        float experiencePoints = 0f;

        public event Action onExperienceGained;

        public void GainExperience(float experience)
        {
            experiencePoints += experience;
            onExperienceGained();
        }

        public float GetExperienceTotal()
        {
            return experiencePoints;
        }

        public void ResetAfterLevelUp(float xpForLastLevel)
        {
            experiencePoints = experiencePoints - xpForLastLevel;
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(experiencePoints);
        }

        public void RestoreFromJToken(JToken state)
        {
            experiencePoints = state.ToObject<float>();
        }
    }
}