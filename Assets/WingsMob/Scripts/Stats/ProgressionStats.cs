using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Global;

namespace WingsMob.Survival.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class ProgressionStats : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;
        Dictionary<CharacterClass, Dictionary<CharacterStats, float[]>> lookupTable = null;

        //public float 
        public float GetStat(CharacterStats stat, CharacterClass characterClass, int level)
        {
            BuildLookup();

            if (!lookupTable[characterClass].ContainsKey(stat))
                return 0;

            float[] levels = lookupTable[characterClass][stat];
            if (levels.Length < level)
            {
                return levels[levels.Length - 1];
            }

            return levels[level - 1];
        }

        private void BuildLookup()
        {
            if (lookupTable != null) return;

            lookupTable = new Dictionary<CharacterClass, Dictionary<CharacterStats, float[]>>();

            foreach (ProgressionCharacterClass progressionClass in characterClasses)
            {
                var statLookupTable = new Dictionary<CharacterStats, float[]>();

                foreach (ProgressionStat progressionStat in progressionClass.stats)
                {
                    statLookupTable[progressionStat.stat] = progressionStat.levels;
                }

                lookupTable[progressionClass.characterClass] = statLookupTable;
            }
        }

        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            [TableList(ShowPaging = true)]
            public ProgressionStat[] stats;
        }

        [System.Serializable]
        class ProgressionStat
        {
            [TableColumnWidth(60)]
            public CharacterStats stat;
            [TableColumnWidth(60)]
            public float[] levels;
        }
    }
}
