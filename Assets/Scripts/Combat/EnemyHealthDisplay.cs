using RPG.Attributes;
using TMPro;
using UnityEngine;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour 
    {
        Fighter fighter;
        Health target;
        TextMeshProUGUI tmp;
        private void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
            tmp = GetComponent<TextMeshProUGUI>();
        }

        private void Update() 
        {
            target = fighter.GetTarget();
            if(target == null)
            {
                tmp.text = "N/A";
            }
            else
            {
                //tmp.text = $"{target.GetPercentage():0}%";
                tmp.text = $"{target.health:0}/{target.GetMaximumHealth():0}";
            }
        }
    }
}