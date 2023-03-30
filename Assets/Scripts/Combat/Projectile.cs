using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

public class Projectile : MonoBehaviour
{
    [SerializeField] float projectileSpeed = 5f;
    Health target = null;
    float damage = 0;

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;
        transform.LookAt(GetAimLocation());
        transform.Translate(Vector3.forward*Time.deltaTime*projectileSpeed);
    }

    public void SetTarget(Health target, float damage)
    {
        this.target = target;
        this.damage = damage;
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
        if(other.GetComponent<Health>() == target)
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
