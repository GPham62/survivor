using UnityEngine;
using WingsMob.Survival.Global;
using static WingsMob.Survival.Stats.ProgressionStats;

namespace WingsMob.Survival.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [SerializeField] private int m_level;
        [SerializeField] private CharacterClass m_characterClass;
        [SerializeField] private ProgressionStats m_progression;

        public float GetBaseStat(CharacterStats stat)
        {
            return m_progression.GetStat(stat, m_characterClass, m_level);
        }

        public int GetLevel() => m_level;

        public void SetLevel(int level) => m_level = level;
    }
}