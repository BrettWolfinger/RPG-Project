using TMPro;
using UnityEngine;

namespace RPG.Attributes
{
    public class ManaDisplay : MonoBehaviour 
    {
        Mana mana;
        TextMeshProUGUI tmp;
        private void Awake()
        {
            mana = GameObject.FindWithTag("Player").GetComponent<Mana>();
            tmp = GetComponent<TextMeshProUGUI>();
        }

        private void Update() 
        {
            //tmp.text = $"{mana.GetPercentage():0}%";
            tmp.text = $"{mana.mana:0}/{mana.GetMaxMana():0}";
        }
    }
}