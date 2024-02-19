using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    public class AbilityData
    {
        public GameObject user {get;}
        public Vector3 targetedPoint {get;set;}
        public IEnumerable<GameObject> targets {get;set;}

        public AbilityData(GameObject user)
        {
            this.user = user;
        }

        public void StartCoroutine(IEnumerator coroutine)
        {
            user.GetComponent<MonoBehaviour>().StartCoroutine(coroutine);
        }
    }
}