using UnityEngine;
using RPG.Movement;
using RPG.Core;
namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        //Cached component variables
        Mover mover;
        Health target;
        ActionScheduler scheduler;
        Animator animator;

        //Serialized Fields
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float weaponDamage = 5f;

        //Other helper variables
        float timeSinceLastAttack = Mathf.Infinity;

        private void Awake() {
            mover = GetComponent<Mover>();
            scheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();
        }
        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (target == null) return;
            if (target.IsDead()) return;

            if (GetIsOutOfRange())
            {
                mover.MoveTo(target.transform.position);
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
            transform.LookAt(target.transform);
            if(timeSinceLastAttack > timeBetweenAttacks)
            {
                TriggerAttackAnimation();
                timeSinceLastAttack = 0;
                //Hit() event triggers
            }
        }

        private void TriggerAttackAnimation()
        {
            animator.ResetTrigger("stopAttack");
            animator.SetTrigger("attack");
        }

        //animation event
        void Hit()
        {
            if (target == null) return;
            target.TakeDamage(weaponDamage);
        }

        private bool GetIsOutOfRange()
        {
            return Vector3.Distance(target.transform.position, transform.position) > weaponRange;
        }

        public void Attack(GameObject combatTarget)
        {
            scheduler.StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            StopAttackAnimation();
            target = null;
            mover.Cancel();
        }

        private void StopAttackAnimation()
        {
            animator.ResetTrigger("attack");
            animator.SetTrigger("stopAttack");
        }
    }
}