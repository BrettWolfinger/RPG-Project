using System;
using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, IJsonSaveable
    {
        bool isDead = false;

        [SerializeField] float health = 100f;

        private void Awake() {
            health = GetComponent<BaseStats>().GetHealth();
        }

        public void TakeDamage(GameObject attacker, float damage)
        {
            //lower bound health to zero
            health = Mathf.Max(health - damage,0);
            print(health);
            if(health == 0)
            {
                Die();
                AwardExperience(attacker);
            }
        }

        private void Die()
        {
            if(isDead) return;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
            GetComponent<NavMeshAgent>().enabled = false;
            if(GetComponent<Collider>()) GetComponent<Collider>().enabled = false;
            isDead = true;
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if(experience == null) return;

            experience.GainExperience(GetComponent<BaseStats>().GetExperienceReward());
        }

        public bool IsDead()
        {
            return isDead;
        }

        public float GetPercentage() 
        {
            return 100 * (health / GetComponent<BaseStats>().GetHealth());
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(health);
        }

        public void RestoreFromJToken(JToken state)
        {
            health = state.ToObject<float>();
            if(health <= 0)
            {
                Die();
            }
        }
    }
}