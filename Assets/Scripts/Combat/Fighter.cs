using UnityEngine;
using UnityEngine.Serialization;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using Newtonsoft.Json.Linq;
using RPG.Attributes;
using GameDevTV.Utils;
using System;
using RPG.Stats;
using System.Collections.Generic;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, IJsonSaveable, IModifierProvider
    {
        //Cached component variables
        Mover mover;
        Health target;
        ActionScheduler scheduler;
        Animator animator;
        BaseStats stats;

        //Serialized Fields
        [SerializeField] float timeBetweenAttacks = 1f;
        [FormerlySerializedAs("handTransform")]
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Weapon_SO defaultWeapon = null;


        //Other helper variables
        float timeSinceLastAttack = Mathf.Infinity;
        LazyValue<Weapon_SO> _currentWeapon;
        public Weapon_SO currentWeapon
        {
            get {return _currentWeapon.value;}
            set {_currentWeapon.value=value;}
        }

        private void Awake() {
            mover = GetComponent<Mover>();
            scheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();
            stats = GetComponent<BaseStats>();
            _currentWeapon = new LazyValue<Weapon_SO>(SetupDefaultWeapon);
        }
        private void Start() {
            _currentWeapon.ForceInit();
        }

        private Weapon_SO SetupDefaultWeapon()
        {
            AttachWeapon(defaultWeapon);
            return defaultWeapon;
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
            AttachWeapon(weapon);
        }

        private void AttachWeapon(Weapon_SO weapon)
        {
            if (weapon == null) return;
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
            float damage = CalculateDamage();
            if (currentWeapon.HasProjectile())
            {
                currentWeapon.LaunchProjectile(rightHandTransform, leftHandTransform, gameObject, target, damage);
            }
            else
            {
                target.TakeDamage(gameObject, damage);
            }
        }

        private float CalculateDamage()
        {
            return stats.GetCharacterStat(CharacterStat.BaseDamage);
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

        public Health GetTarget()
        {
            return target;
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
            Weapon_SO weapon = UnityEngine.Resources.Load<Weapon_SO>(weaponName);
            EquipWeapon(weapon);
        }

        public IEnumerable<float> GetAdditiveModifiers(CharacterStat stat)
        {
            if(stat == CharacterStat.BaseDamage)
            {
                yield return currentWeapon.GetWeaponDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(CharacterStat stat)
        {
            if(stat == CharacterStat.BaseDamage)
            {
                yield return currentWeapon.GetWeaponPercentage();
            }
        }
    }
}