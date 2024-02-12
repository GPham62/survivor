using DarkTonic.PoolBoss;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Global;
using WingsMob.Survival.Model;

namespace WingsMob.Survival.Combat
{
    public class Bonemerang : Weapon
    {
        private float m_damage, m_rechargeTime, m_projectileSpeed, m_projectileAmount, m_aoe, m_range, m_duration;
        private bool m_isUltimate;
        public override void Init(Action<string> callback)
        {
            m_tempThrowDirs = new List<Vector2>();
            m_isUltimate = false;
            m_tweenAttackId = name;
            base.Init(callback);
            UpdateVariables();
            StartThrowingSequence();
        }

        private void UpdateVariables()
        {
            m_projectileSpeed = m_weaponStats.GetWeaponParam(WeaponParam.ProjectileSpeed) / 100;
            m_damage = m_weaponStats.GetWeaponParam(WeaponParam.Damage) / 100;
            m_rechargeTime = m_weaponStats.GetWeaponParam(WeaponParam.RechargeTime);
            m_aoe = m_weaponStats.GetWeaponParam(WeaponParam.AreaOfEffect) / 100;
            m_range = m_weaponStats.GetWeaponParam(WeaponParam.Range);
            if (m_isUltimate)
            {
                m_duration = m_weaponStats.GetWeaponParam(WeaponParam.Duration);
            }
            else
                m_projectileAmount = m_weaponStats.GetWeaponParam(WeaponParam.ProjectileAmount);
        }

        private void StartThrowingSequence()
        {
            Sequence boneThrowingSequence = DOTween.Sequence();
            boneThrowingSequence.AppendCallback(() =>
            {
                ThrowBones();
            })
                .AppendInterval(m_rechargeTime * (1 - m_levelManager.playerController.fighter.cooldownReduction))
                .SetId(name)
                .SetLoops(-1, LoopType.Restart);
        }

        private void ThrowBones()
        {
            if (m_isUltimate)
            {
                Transform boneLeftTransform = PoolBoss.Spawn(m_weaponStats.GetWeaponProjectile().transform, transform.position, Quaternion.identity, transform);
                boneLeftTransform.GetComponent<BonemerangProjectile>().InitUltimateSpiral(
                m_projectileSpeed,
                m_levelManager.playerController.fighter.Damage * m_damage,
                m_duration * (1 + m_levelManager.playerController.fighter.weaponDuration),
                m_aoe * (1 + m_levelManager.playerController.fighter.weaponAoe),
                m_range,
                true);

                Transform boneRightTransform = PoolBoss.Spawn(m_weaponStats.GetWeaponProjectile().transform, transform.position, Quaternion.identity, transform);
                boneRightTransform.GetComponent<BonemerangProjectile>().InitUltimateSpiral(
                m_projectileSpeed,
                m_levelManager.playerController.fighter.Damage * m_damage,
                m_duration * (1 + m_levelManager.playerController.fighter.weaponDuration),
                m_aoe * (1 + m_levelManager.playerController.fighter.weaponAoe),
                m_range,
                false);
            }
            else
            {
                GenerateThrowDirections();
                for (int i = 0; i < m_projectileAmount; i++)
                {
                    Transform boneTransform = PoolBoss.SpawnInPool(m_weaponStats.GetWeaponProjectile().transform, transform.position, Quaternion.identity);
                    boneTransform.GetComponent<BonemerangProjectile>().Init(
                    m_tempThrowDirs[i],
                    m_projectileSpeed,
                    m_levelManager.playerController.fighter.Damage * m_damage,
                    m_aoe * (1 + m_levelManager.playerController.fighter.weaponAoe),
                    m_range
                    );
                }
            }
        }

        private List<Vector2> m_tempThrowDirs;

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

        public override void NormalLevelUp()
        {
            base.NormalLevelUp();
            UpdateVariables();
        }

        public override void UltimateUpgrade()
        {
            base.UltimateUpgrade();
            m_isUltimate = true;
            UpdateVariables();
        }
    }
}