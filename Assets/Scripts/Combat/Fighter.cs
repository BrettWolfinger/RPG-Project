using UnityEngine;
using UnityEngine.Serialization;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using Newtonsoft.Json.Linq;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, IJsonSaveable
    {
        //Cached component variables
        Mover mover;
        Health target;
        ActionScheduler scheduler;
        Animator animator;

        //Serialized Fields
        [SerializeField] float timeBetweenAttacks = 1f;
        [FormerlySerializedAs("handTransform")]
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Weapon_SO defaultWeapon = null;


        //Other helper variables
        float timeSinceLastAttack = Mathf.Infinity;
        Weapon_SO currentWeapon = null;

        private void Awake() {
            mover = GetComponent<Mover>();
            scheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();
            if (currentWeapon == null)
            {
                EquipWeapon(defaultWeapon);
            }
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

        public void EquipWeapon(Weapon_SO weapon)
        {
            currentWeapon = weapon;
            if(weapon == null) return;
            weapon.Spawn(rightHandTransform, leftHandTransform, animator);
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
            if(currentWeapon.HasProjectile())
            {
                currentWeapon.LaunchProjectile(rightHandTransform, leftHandTransform, target);
            }
            else
            {
                target.TakeDamage(currentWeapon.GetWeaponDamage());
            }
        }

        void Shoot()
        {
            Hit();
        }

        private bool GetIsOutOfRange()
        {
            return Vector3.Distance(target.transform.position, transform.position) > currentWeapon.GetWeaponRange();
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

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(currentWeapon.name);
        }

        public void RestoreFromJToken(JToken state)
        {
            string weaponName = state.ToObject<string>();
            Weapon_SO weapon = Resources.Load<Weapon_SO>(weaponName);
            EquipWeapon(weapon);
        }
    }
}