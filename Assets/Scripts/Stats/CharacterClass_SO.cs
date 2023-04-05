using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "CharacterClass", menuName = "Classes/Make New Character Class", order = 0)]
    public class CharacterClass_SO : ScriptableObject
    {
        [SerializeField] List<ProgressionStat> statData = new List<ProgressionStat>();
        Dictionary<CharacterStat, float[]> statLookupTable = null;
        void BuildLookup()
        {
           if(statLookupTable!=null) return;
           statLookupTable = new Dictionary<CharacterStat, float[]>();
           foreach(ProgressionStat progStat in statData)
           {
               statLookupTable[progStat.stat] = progStat.levels;
           }
        }
        
        public float GetCharacterStat(CharacterStat stat, int level)
        {
            BuildLookup();
            if(!statLookupTable.ContainsKey(stat))
            {
                Debug.Log($"{name} does not contain an entry for {stat}");
                return 1;
            }
            if(statLookupTable[stat].Length<level)
            {
                Debug.Log($"{name}'s {stat} does not contain level {level}");
                return 1;
            }
            return statLookupTable[stat][level-1];
        }

        public float GetLevels(CharacterStat stat)
        {
            BuildLookup();

            float[] levels = statLookupTable[stat];
            return levels.Length;
        }


        [System.Serializable]
        class ProgressionStat
        {
            public CharacterStat stat;
            public float[] levels;
        }
    }
}