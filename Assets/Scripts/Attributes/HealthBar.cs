using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health healthComponent = null;
        [SerializeField] RectTransform foreground = null;
        [SerializeField] Canvas rootCanvas = null;

        public void UpdateHealthBar()
        {
            print(healthComponent.GetFraction());
            if(Mathf.Approximately(healthComponent.GetFraction(),0)
            || Mathf.Approximately(healthComponent.GetFraction(),1))
            {
                rootCanvas.enabled = false;
                return;
            }
            
            foreground.localScale = new Vector3(healthComponent.GetFraction(), 1, 1);
        }
    }
}
