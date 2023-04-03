using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "CharacterClass", menuName = "Classes/Make New Character Class", order = 0)]
    public class CharacterClass_SO : ScriptableObject
    {
        [SerializeField] float[] health;

        public float GetHealth(int level)
        {
            return health[level-1];
        }
    }
}