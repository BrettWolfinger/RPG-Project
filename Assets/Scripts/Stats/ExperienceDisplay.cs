using TMPro;
using UnityEngine;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour 
    {
        Experience xp;
        BaseStats baseStats;
        TextMeshProUGUI tmp;

        private void Awake()
        {
            xp = GameObject.FindWithTag("Player").GetComponent<Experience>();
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
            tmp = GetComponent<TextMeshProUGUI>();
        }

        private void Update() 
        {
            tmp.text = $"{xp.GetExperienceTotal():0}";
        }
    }
}