using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Core
{
    public class Health : MonoBehaviour, ISaveable
    {
        bool isDead = false;

        [SerializeField] float health = 100f;

        public void TakeDamage(float damage)
        {
            //lower bound health to zero
            health = Mathf.Max(health - damage,0);
            print(health);
            if(health == 0)
            {
                Die();
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

        public bool IsDead()
        {
            return isDead;
        }

        public object CaptureState() 
        {
            return health;
        }

        public void RestoreState(object state)
        {
            health = (float)state;
            if(health <= 0)
            {
                Die();
            }
        }

    }
}