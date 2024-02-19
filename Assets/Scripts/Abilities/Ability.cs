using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using GameDevTV.Inventories;
using RPG.Attributes;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "MyAbility", menuName = "RPG/Ability/Ability", order = 0)]
    public class Ability : ActionItem
    {
        [SerializeField] TargetingStrategy targetingStrategy;
        [SerializeField] FilterStrategy[] filterStrategies;
        [SerializeField] EffectStrategy[] effectStrategies;
        [SerializeField] float cooldownTime = 0;
        [SerializeField] float manaCost = 0;

        public override void Use(GameObject user)
        {
            //ability is still on cooldown
            if(user.GetComponent<CooldownStore>().GetTimeRemaining(this) > 0)
            {
                return;
            }
            //user does not have enough mana to use ability
            if(user.GetComponent<Mana>().mana <= manaCost)
            {
                return;
            }
            AbilityData data = new AbilityData(user);
            targetingStrategy.StartTargeting(data,
                () => {
                    TargetAcquired(data);
                });
    }

        private void TargetAcquired(AbilityData data)
        {
            //target has been  selected, use mana and start cooldown
            if(data.user.GetComponent<Mana>().UseMana(manaCost)) return;
            data.user.GetComponent<CooldownStore>().StartCooldown(this, cooldownTime);


            foreach (FilterStrategy filterStrategy in filterStrategies)
            {
                data.targets = filterStrategy.Filter(data.targets);
            }
            foreach(EffectStrategy effect in effectStrategies)
            {
                effect.StartEffect(data, EffectFinished);
            }
        }

        private void EffectFinished()
        {
              
        }
    }
}

