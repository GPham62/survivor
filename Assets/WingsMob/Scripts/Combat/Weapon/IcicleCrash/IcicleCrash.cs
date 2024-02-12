using DarkTonic.PoolBoss;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Global;
using WingsMob.Survival.Model;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Combat
{
    public class IcicleCrash : Weapon
    {
        private float m_damage, m_duration, m_rechargeTime, m_projectileSpeed, m_projectileAmount;
        private Transform m_tempEnemy;
        //private bool m_isKnockBack;

        public override void Init(Action<string> callback)
        {
            m_tempThrowDirs = new List<Vector2>();
            //m_isKnockBack = false;
            m_tweenAttackId = name;
            base.Init(callback);
            UpdateVariables();
            StartThrowingSequence();
        }

        public override void NormalLevelUp()
        {
            base.NormalLevelUp();
            UpdateVariables();
        }

        public override void UltimateUpgrade()
        {
            base.UltimateUpgrade();
            UpdateVariables();
            //m_isKnockBack = true;
        }

        private void UpdateVariables()
        {
            m_projectileSpeed = m_weaponStats.GetWeaponParam(WeaponParam.ProjectileSpeed) / 100;
            m_damage = m_weaponStats.GetWeaponParam(WeaponParam.Damage) / 100;
            m_duration = m_weaponStats.GetWeaponParam(WeaponParam.Duration);
            m_rechargeTime = m_weaponStats.GetWeaponParam(WeaponParam.RechargeTime);
            m_projectileAmount = m_weaponStats.GetWeaponParam(WeaponParam.ProjectileAmount);
        }

        private void StartThrowingSequence()
        {
            Sequence iceThrowingSequence = DOTween.Sequence();
            iceThrowingSequence.AppendCallback(() =>
            {
                ThrowIcicles();
            })
                .AppendInterval(m_rechargeTime * (1 - m_levelManager.playerController.fighter.cooldownReduction))
                .SetId(name)
                .SetLoops(-1, LoopType.Restart);
        }

        private List<Vector2> m_tempThrowDirs;

        private void ThrowIcicles()
        {
            GenerateThrowDirections();
            for (int i = 0; i < m_projectileAmount; i++)
            {
                Transform icicleTrans = PoolBoss.SpawnInPool(m_weaponStats.GetWeaponProjectile().transform, transform.position, Quaternion.identity);
                icicleTrans.GetComponent<Icicle>().Init(
                    m_tempThrowDirs[i],
                    m_projectileSpeed,
                    m_levelManager.playerController.fighter.Damage * m_damage,
                    m_duration * (1 + m_levelManager.playerController.fighter.weaponDuration));
            }
        }

        private void GenerateThrowDirections()
        {
            m_tempThrowDirs.Clear();
            m_tempThrowDirs.Add(m_levelManager.playerController.mover.GetMoveDirection());
            if (m_projectileAmount > 1)
            { 
                float angleDiff = 360 / m_projectileAmount;
                for (int i = 1; i <= m_projectileAmount; i++)
                {
                    m_tempThrowDirs.Add(Quaternion.AngleAxis(angleDiff, Vector3.forward) * m_tempThrowDirs[m_tempThrowDirs.Count - 1]);
                }
            }
        }
    }
}