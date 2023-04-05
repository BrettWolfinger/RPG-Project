using TMPro;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour 
    {
        Health health;
        TextMeshProUGUI tmp;
        private void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
            tmp = GetComponent<TextMeshProUGUI>();
        }

        private void Update() 
        {
            //tmp.text = $"{health.GetPercentage():0}%";
            tmp.text = $"{health.GetHealth():0}/{health.GetMaximumHealth():0}";
        }
    }
}