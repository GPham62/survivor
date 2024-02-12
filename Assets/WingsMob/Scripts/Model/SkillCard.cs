using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WingsMob.Survival.Model
{
    public abstract class SkillCard : MonoBehaviour
    {
        private bool m_canLevelUp = true;
        public virtual bool CanLevelUp
        {
            get { return m_canLevelUp; }
            set { m_canLevelUp = value; }
        }

        public float dropChance;

        public virtual int GetId() { return -99; }

        public virtual int GetCardLevel() => 0;
    }
}
