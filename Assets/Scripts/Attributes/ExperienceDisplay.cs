using TMPro;
using UnityEngine;

namespace RPG.Attributes
{
    public class ExperienceDisplay : MonoBehaviour 
    {
        Experience xp;
        private void Awake()
        {
            xp = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        private void Update() 
        {
            GetComponent<TextMeshProUGUI>().text = $"{xp.GetExperience():0}";
        }
    }
}