using UnityEngine;

namespace RPG.Attributes
{
    public class Experience : MonoBehaviour 
    {
        float experiencePoints = 0f;

        public void GainExperience(float experience)
        {
            experiencePoints += experience;
        }

        public float GetExperience()
        {
            return experiencePoints;
        }
    }
}