using System;
using GameDevTV.Utils;
using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] UnityEvent<float> takeDamage;
        [SerializeField] UnityEvent onDie;
        bool isDead = false;
        LazyValue<float> _health;
        public float health
        {
            get {return _health.value;}
            set {_health.value=value;}
        }
        BaseStats stats;

        private void Awake() {
            stats = GetComponent<BaseStats>();
            _health = new LazyValue<float>(GetIntitialHealth);
        }

        private float GetIntitialHealth()
        {
            return stats.GetCharacterStat(CharacterStat.Health);
        }

        private void Start() 
        {
            _health.ForceInit();
        }

        public void TakeDamage(GameObject attacker, float damage)
        {
            //lower bound health to zero
            health = Mathf.Max(health - damage,0);

            takeDamage.Invoke(damage);

            if(health == 0)
            {
                onDie.Invoke();
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

            experience.GainExperience(stats.GetCharacterStat(CharacterStat.ExperienceReward));
        }

        private void OnEnable() {
            if(stats != null)
            {
                stats.onLevelUp += OnLevelUp;
            }
        }

        private void OnLevelUp()
        {
            health = stats.GetCharacterStat(CharacterStat.Health);
        }

        private void OnDisable() {
            if(stats != null)
            {
                stats.onLevelUp -= OnLevelUp;
            }
        }

        public bool IsDead()
        {
            return isDead;
        }

        public float GetHealth()
        {
            return health;
        }
        public float GetMaximumHealth()
        {
            return stats.GetCharacterStat(CharacterStat.Health);
        }

        public float GetPercentage() 
        {
            return 100 * GetFraction();
        }

        public float GetFraction()
        {
            return health / stats.GetCharacterStat(CharacterStat.Health);
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