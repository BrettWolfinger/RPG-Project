using UnityEngine;
using UnityEngine.AI;

namespace RPG.Combat
{
    public class Health : MonoBehaviour 
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
            GetComponent<NavMeshAgent>().enabled = false;
            GetComponent<Collider>().enabled = false;
            isDead = true;
        }

        public bool IsDead()
        {
            return isDead;
        }
    }
}