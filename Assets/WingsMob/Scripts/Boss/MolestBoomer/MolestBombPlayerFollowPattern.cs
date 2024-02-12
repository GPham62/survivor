using DarkTonic.PoolBoss;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Controller;

namespace WingsMob.Survival.Boss.Skill
{
    public class MolestBombPlayerFollowPattern : MolestBombPattern
    {
        [SerializeField] private Transform m_bossTransform;
        public override float ThrowBomb(Transform bombPrefab, float damage)
        {
            
            return 0f;
        }
    }
}
