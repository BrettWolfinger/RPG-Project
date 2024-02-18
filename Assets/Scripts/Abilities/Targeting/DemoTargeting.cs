using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "DemoTargeting", menuName = "RPG/Ability/Targeting/Demo", order = 0)]
    public class DemoTargeting : TargetingStrategy
    {
        public override void StartTargeting(GameObject user, Action<IEnumerable<GameObject>> finished)
        {
            Debug.Log("Demo targeting started");
            finished(null);
        }
    }
}