using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float projectileSpeed = 5f;
        [SerializeField] bool isHoming = false;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeTime = 10;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lifeAfterImpact = 2;
        Health target = null;
        GameObject attacker = null;
        float damage = 0;

        private void Start() 
        {
            transform.LookAt(GetAimLocation());
        }

        // Update is called once per frame
        void Update()
        {
            if (target == null) return;
            if(isHoming && !target.IsDead()) transform.LookAt(GetAimLocation());
            transform.Translate(Vector3.forward*Time.deltaTime*projectileSpeed);
        }

        public void SetTarget(Health target, GameObject attacker, float damage)
        {
            this.target = target;
            this.damage = damage;
            this.attacker = attacker;

            Destroy(gameObject, maxLifeTime);
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if(targetCapsule == null)
            {
                return target.transform.position;
            }
            return target.transform.position + Vector3.up * (targetCapsule.height/2);
        }

        private void OnTriggerEnter(Collider other) {
            Health otherHealth = other.GetComponent<Health>();
            if(otherHealth == target && !otherHealth.IsDead())
            {
                target.TakeDamage(attacker, damage);
                projectileSpeed = 0;
                if(hitEffect != null)
                {
                    Instantiate(hitEffect,GetAimLocation(),transform.rotation);
                }
                foreach (GameObject toDestroy in destroyOnHit)
                {
                    Destroy(toDestroy);
                }

                Destroy(gameObject, lifeAfterImpact);
            }
        }
    }
}