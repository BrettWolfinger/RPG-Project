using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Spawn Target Prefab Effect", menuName = "RPG/Ability/Effects/SpawnTargetPrefab", order = 0)]

    public class SpawnTargetPrefabEffect : EffectStrategy
    {
        [SerializeField] Transform prefabToSpawn;
        //won't destroy unless set in editor
        [SerializeField] float destroyDelay = -1;

        public override void StartEffect(AbilityData data, Action finished)
        {
            data.StartCoroutine(Effect(data, finished));
        }

        private IEnumerator Effect(AbilityData data, Action finished)
        {
            Transform instance = Instantiate(prefabToSpawn);
            instance.position = data.targetedPoint;
            if(destroyDelay > 0)
            {
                yield return new WaitForSeconds(destroyDelay);
                Destroy(instance.gameObject);
            }
            finished();
        }
    }
}