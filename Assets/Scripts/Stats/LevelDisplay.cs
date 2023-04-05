using TMPro;
using UnityEngine;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour 
    {
        BaseStats baseStats;
        TextMeshProUGUI tmp;
        private void Awake()
        {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
            tmp = GetComponent<TextMeshProUGUI>();
        }

        private void Update() 
        {
            tmp.text = $"{baseStats.GetLevel():0}";
        }
    }
}