using System;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Health Effect", menuName = "RPG/Ability/Effects/Damage", order = 0)]

    public class HealthEffect : EffectStrategy
    {
        //Negative amount for taking damage
        [SerializeField] float healthChange;

        public override void StartEffect(AbilityData data, Action finished)
        {
            foreach(GameObject target in data.targets)
            {
                Health health = target.GetComponent<Health>();
                if(health)
                {
                    health.TakeDamage(data.user, healthChange);
                }
            }
            finished();
        }
    }
}