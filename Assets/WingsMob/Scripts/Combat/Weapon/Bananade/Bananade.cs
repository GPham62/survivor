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
    public class Bananade : Weapon
    {
        private float m_damage, m_rechargeTime, m_projectileSpeed, m_projectileAmount, m_aoe, m_range;
        private bool m_isUltimate;
        public override void Init(Action<string> callback)
        {
            m_tweenAttackId = name;
            base.Init(callback);
            UpdateVariables();
            StartThrowingSequence();
        }

        public override void ResetStats()
        {
            DOTween.Kill(name, false);
            StartThrowingSequence();
        }

        private void UpdateVariables()
        {
            m_damage = m_weaponStats.GetWeaponParam(WeaponParam.Damage) / 100;
            m_projectileAmount = m_weaponStats.GetWeaponParam(WeaponParam.ProjectileAmount);
            m_rechargeTime = m_weaponStats.GetWeaponParam(WeaponParam.RechargeTime);
            m_aoe = m_weaponStats.GetWeaponParam(WeaponParam.AreaOfEffect) / 100;
            m_projectileSpeed = m_weaponStats.GetWeaponParam(WeaponParam.ProjectileSpeed) / 100;
            if (m_isUltimate)
                m_range = m_weaponStats.GetWeaponParam(WeaponParam.Range) / 100;
        }

        private float m_throwDelay = 0.2f;

        private void StartThrowingSequence()
        {
            Sequence bananaThrowingSequence = DOTween.Sequence();
            for (int i = 0; i < m_projectileAmount; i++)
            {
                bananaThrowingSequence
                    .AppendCallback(() =>
                {
                    ThrowBanana();
                })
                    .AppendInterval(m_throwDelay);
            }
            bananaThrowingSequence
                .AppendInterval(m_rechargeTime * (1 - m_levelManager.playerController.fighter.cooldownReduction) - m_throwDelay * m_projectileAmount)
                .SetId(name)
                .SetLoops(-1, LoopType.Restart);
        }

        private Transform m_tempEnemy;

        private void ThrowBanana()
        {
             if (m_isUltimate)
            {
                m_tempEnemy = m_levelManager.enemyDetector.GetFarthestEnemyOnScreen();
                Transform bananaTransform = PoolBoss.SpawnInPool(m_weaponStats.GetWeaponProjectile().transform, transform.position, Quaternion.identity);
                Vector2 targetPos = m_tempEnemy == null ? GetRandomPosAroundPlayer() : (Vector2)m_tempEnemy.position;
                Vector2 throwDirection = targetPos - (Vector2) transform.position;
                bananaTransform.GetComponent<BananadeProjectile>().InitUltimate(
                    (Vector2)transform.position + throwDirection.normalized * m_range,
                    m_projectileSpeed,
                    m_levelManager.playerController.fighter.Damage * m_damage,
                    m_aoe * (1 + m_levelManager.playerController.fighter.weaponAoe),
                    m_weaponStats.GetWeaponProjectileParticle());
            }
            else
            {
                m_tempEnemy = m_levelManager.enemyDetector.GetNearestEnemyOnScreen();
                Transform bananaTransform = PoolBoss.SpawnInPool(m_weaponStats.GetWeaponProjectile().transform, transform.position, Quaternion.identity);
                bananaTransform.GetComponent<BananadeProjectile>().Init(
                    targetPos: m_tempEnemy == null ? GetRandomPosAroundPlayer() : (Vector2)m_tempEnemy.position,
                    m_projectileSpeed,
                    m_levelManager.playerController.fighter.Damage * m_damage,
                    m_aoe * (1 + m_levelManager.playerController.fighter.weaponAoe),
                    m_weaponStats.GetWeaponProjectileParticle());
            }
        }

        private Vector2 GetRandomPosAroundPlayer()
        {
            Vector2 randomPos = new Vector2(transform.position.x + UnityEngine.Random.Range(-5f, 5f), transform.position.y + UnityEngine.Random.Range(-8.5f, 8.5f));
            if (Mathf.Abs(randomPos.x) < 0.1 && Mathf.Abs(randomPos.y) < 0.1)
                return Vector2.one;
            return randomPos;
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
            ResetStats();
        }
    }
}