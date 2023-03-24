using UnityEngine;
using RPG.Movement;
using RPG.Core;
namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        //Cached component variables
        Mover mover;
        Transform target;
        ActionScheduler scheduler;
        Animator animator;

        //Serialized Fields
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float weaponDamage = 5f;

        //Other helper variables
        float timeSinceLastAttack = 0;

        private void Awake() {
            mover = GetComponent<Mover>();
            scheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();
        }
        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (!target) return;

            if (GetIsOutOfRange())
            {
                mover.MoveTo(target.position);
            }
            else
            {
                mover.Cancel();
                //in range of target, start attacking!
                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            if(timeSinceLastAttack > timeBetweenAttacks)
            {
                animator.SetTrigger("attack");
                timeSinceLastAttack = 0;
                //Hit() event triggers
            }
        }

        //animation event
        void Hit()
        {
            target.GetComponent<Health>().TakeDamage(weaponDamage);
        }

        private bool GetIsOutOfRange()
        {
            return Vector3.Distance(target.position, transform.position) > weaponRange;
        }

        public void Attack(CombatTarget combatTarget)
        {
            scheduler.StartAction(this);
            target = combatTarget.transform;
        }

        public void Cancel()
        {
            target = null;
        }
    }
}