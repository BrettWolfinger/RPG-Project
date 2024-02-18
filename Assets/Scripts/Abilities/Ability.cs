using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using GameDevTV.Inventories;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "MyAbility", menuName = "RPG/Ability/Ability", order = 0)]
    public class Ability : ActionItem
    {
        [SerializeField] TargetingStrategy targetingStrategy;
        [SerializeField] FilterStrategy[] filterStrategies;

        public override void Use(GameObject user)
        {
            targetingStrategy.StartTargeting(user, TargetAcquired);
        }

        private void TargetAcquired(IEnumerable<GameObject> targets)
        {
            foreach (FilterStrategy filterStrategy in filterStrategies)
            {
                targets = filterStrategy.Filter(targets);
            }
            foreach(var target in targets)
            {
                Debug.Log(target);
            }
        }
    }
}

