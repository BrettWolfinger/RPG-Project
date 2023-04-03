using RPG.Attributes;
using TMPro;
using UnityEngine;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour 
    {
        Fighter fighter;
        Health target;
        private void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        private void Update() 
        {
            target = fighter.GetTarget();
            if(target == null)
            {
                GetComponent<TextMeshProUGUI>().text = "N/A";
            }
            else
            {
                GetComponent<TextMeshProUGUI>().text = $"{target.GetPercentage():0}%";
            }
        }
    }
}