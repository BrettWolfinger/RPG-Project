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
using GameDevTV.Inventories;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, IJsonSaveable
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
        Equipment equipment;
        float timeSinceLastAttack = Mathf.Infinity;
        Weapon_SO currentWeapon_SO;
        LazyValue<Weapon> _currentWeapon;
        public Weapon currentWeapon
        {
            get {return _currentWeapon.value;}
            set {_currentWeapon.value=value;}
        }

        private void Awake() {
            mover = GetComponent<Mover>();
            scheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();
            stats = GetComponent<BaseStats>();
            currentWeapon_SO = defaultWeapon;
            _currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
            equipment = GetComponent<Equipment>();
            if (equipment)
            {
                equipment.equipmentUpdated += UpdateWeapon;
            }
        }

        private void Start() {
            AttachWeapon(currentWeapon_SO);
            _currentWeapon.ForceInit();
        }

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target == null) return;
            if (target.IsDead()) return;

            if (GetIsOutOfRange(target.transform))
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
            currentWeapon_SO = weapon;
            currentWeapon = AttachWeapon(weapon);
        }

        private void UpdateWeapon()
        {
            var weapon = equipment.GetItemInSlot(EquipLocation.Weapon) as Weapon_SO;

            if(weapon == null)
            {
                EquipWeapon(defaultWeapon);
            }
            else
            {
                EquipWeapon(weapon);   
            }
        }

        private Weapon AttachWeapon(Weapon_SO weapon)
        {
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
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

            if(currentWeapon != null)
            {
                currentWeapon.OnHit();
            }

            if (currentWeapon_SO.HasProjectile())
            {
                currentWeapon_SO.LaunchProjectile(rightHandTransform, leftHandTransform, gameObject, target, damage);
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

        private bool GetIsOutOfRange(Transform targetTransform)
        {
            return Vector3.Distance(targetTransform.position, transform.position) > currentWeapon_SO.GetWeaponRange();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) { return false; }
            if (!GetComponent<Mover>().CanMoveTo(combatTarget.transform.position) &&
                GetIsOutOfRange(combatTarget.transform))
            { 
                return false; 
            }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
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
            return JToken.FromObject(currentWeapon_SO.name);
        }

        public void RestoreFromJToken(JToken state)
        {
            string weaponName = state.ToObject<string>();
            Weapon_SO weapon = UnityEngine.Resources.Load<Weapon_SO>(weaponName);
            EquipWeapon(weapon);
        }
    }
}