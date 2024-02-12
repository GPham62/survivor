using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.PoolBoss;
using WingsMob.Survival.Global;
using WingsMob.Survival.Utils;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Model;
using System;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace WingsMob.Survival.Combat
{
    public class RockThrow : BasicWeapon
    {
        [Title("Rock Throw Stats")]
        [ReadOnly] [SerializeField] private float m_throwSpeed;
        [ReadOnly] [SerializeField] private int m_projectileAmount;
        [ReadOnly] [SerializeField] private float m_rockDamage;

        public override void Init(Action<string> callback)
        {
            base.Init(callback);
            UpdateVariables(true);
        }

        public override void NormalLevelUp()
        {
            base.NormalLevelUp();
            UpdateVariables(true);
        }

        public override void UltimateUpgrade()
        {
            base.UltimateUpgrade();
            UpdateVariables(false);
        }

        private void UpdateVariables(bool isNormalUpdate)
        {
            if (isNormalUpdate)
                m_projectileAmount = (int)m_weaponStats.GetWeaponParam(WeaponParam.ProjectileAmount);

            m_throwSpeed = m_weaponStats.GetWeaponParam(WeaponParam.ProjectileSpeed) / 100;
            m_rockDamage = m_weaponStats.GetWeaponParam(WeaponParam.Damage) / 100;
        }

        private Transform m_tempEnemy;

        protected override void NormalAttack()
        {
            StartCoroutine(IEThrowRock());
        }

        private float m_throwDelay = 0.1f;
        private IEnumerator IEThrowRock()
        {
            int projectileLeft = 0;
            while (projectileLeft < m_projectileAmount)
            {
                Transform rockTrans = PoolBoss.SpawnInPool(m_weaponStats.GetWeaponProjectile().transform, transform.position, Quaternion.identity);
                m_tempEnemy = m_levelManager.enemyDetector.GetNearestEnemyOnScreen();
                rockTrans.GetComponent<Rock>().Init(
                    m_tempEnemy != null ? (Vector2) m_tempEnemy.transform.position : GetRandomPosAroundPlayer(),
                    m_throwSpeed,
                    m_levelManager.playerController.fighter.Damage * m_rockDamage);

                projectileLeft++;
                yield return new WaitForSeconds(m_throwDelay);
            }
        }

        private Vector2 GetRandomPosAroundPlayer()
        {
            Vector2 randomPos = new Vector2(transform.position.x + UnityEngine.Random.Range(-5f, 5f), transform.position.y + UnityEngine.Random.Range(-8.5f, 8.5f));
            if (Mathf.Abs(randomPos.x) < 0.1 && Mathf.Abs(randomPos.y) < 0.1)
                return Vector2.one;
            return randomPos;
        }

        protected override void UltiAttack()
        {
            Transform rockTrans = PoolBoss.SpawnInPool(m_weaponStats.GetWeaponProjectile().transform, transform.position, Quaternion.identity);
            m_tempEnemy = m_levelManager.enemyDetector.GetNearestEnemyOnScreen();
            rockTrans.GetComponent<Rock>().Init(
                m_tempEnemy != null ? (Vector2)m_tempEnemy.transform.position : GetRandomPosAroundPlayer(),
                m_throwSpeed,
                m_levelManager.playerController.fighter.Damage * m_rockDamage,
                m_weaponStats.GetWeaponProjectileParticle());
        }
    }
}