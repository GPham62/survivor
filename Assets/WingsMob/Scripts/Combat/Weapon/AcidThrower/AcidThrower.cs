using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.PoolBoss;
using WingsMob.Survival.Utils;
using WingsMob.Survival;
using WingsMob.Survival.Model;
using System;
using WingsMob.Survival.Global;
using DG.Tweening;
using WingsMob.Survival.Controller;

namespace WingsMob.Survival.Combat
{
    public class AcidThrower : Weapon
    {
        [SerializeField] private float m_radius = 3.0f;
        [SerializeField] private float m_throwTime = 0.3f;

        private float m_projectileDamage, m_rechargeTime, m_slowEffect, m_projectileAmount, m_effectDuration, m_aoe;
        private List<WeaponProjectile> m_projectileList;
        private Vector2 m_projectileOriginalScale;

        public override void Init(Action<string> callback)
        {
            m_tweenAttackId = name;
            base.Init(callback);
            m_projectileList = new List<WeaponProjectile>();
            m_projectileOriginalScale = m_weaponStats.GetWeaponProjectile().transform.localScale;
            UpdateVariables();
            SpawnAcidBottles();
            StartThrowingAcidSequence();
        }

        public override void ResetStats()
        {
            DOTween.Kill(name, false);
            StartThrowingAcidSequence();
        }

        private void UpdateVariables()
        {
            m_projectileDamage = m_weaponStats.GetWeaponParam(WeaponParam.Damage) / 100;
            m_rechargeTime = m_weaponStats.GetWeaponParam(WeaponParam.RechargeTime);
            m_slowEffect = m_weaponStats.GetWeaponParam(WeaponParam.SlowEffect);
            m_projectileAmount = m_weaponStats.GetWeaponParam(WeaponParam.ProjectileAmount);
            m_effectDuration = m_weaponStats.GetWeaponParam(WeaponParam.Duration);
            m_aoe = m_weaponStats.GetWeaponParam(WeaponParam.AreaOfEffect) / 100;
        }

        private void SpawnAcidBottles()
        {
            for (int i = m_projectileList.Count; i < m_projectileAmount; i++)
            {
                WeaponProjectile newProjectile = Instantiate(m_weaponStats.GetWeaponProjectile(), transform);
                newProjectile.gameObject.SetActive(false);
                newProjectile.transform.localScale = Vector2.zero;
                m_projectileList.Add(newProjectile);
            }
        }

        private void StartThrowingAcidSequence()
        {
            Sequence acidThrowSequence = DOTween.Sequence();
            acidThrowSequence.AppendCallback(() =>
            {
                ThrowAcidBottles();
            })
                .AppendInterval(m_rechargeTime * (1 - m_levelManager.playerController.fighter.cooldownReduction))
                .SetId(name)
                .SetLoops(-1, LoopType.Restart);
        }

        private void ThrowAcidBottles()
        {
            m_throwPositions = GenerateThrowPositions(m_projectileAmount);
            for (int i = 0; i < m_projectileAmount; i ++)
            {
                Transform projectileTransform = m_projectileList[i].transform;
                projectileTransform.gameObject.SetActive(true);
                projectileTransform.rotation = m_throwPositions[i].y < 0 ?
                    Quaternion.Euler(0, 0, UnityEngine.Random.Range(90, 270)) : Quaternion.Euler(0, 0, UnityEngine.Random.Range(-90, 90));

                projectileTransform.DOScale(m_aoe > 1 ? m_projectileOriginalScale * m_aoe * (1 + m_levelManager.playerController.fighter.weaponAoe) / 2 : m_projectileOriginalScale, m_throwTime).SetEase(Ease.OutQuint);
                projectileTransform.DOMove(m_throwPositions[i], m_throwTime).SetEase(Ease.Linear).OnComplete(() => 
                {
                    SpawnAcidField(projectileTransform.position);
                    projectileTransform.localPosition = Vector2.zero;
                    projectileTransform.localScale = Vector2.zero;
                    projectileTransform.gameObject.SetActive(false);
                });
            }
        }

        private List<Vector2> m_throwPositions;
        
        private List<Vector2> GenerateThrowPositions(float amount)
        {
            float angle = UnityEngine.Random.Range(0f, 360f); // start angle

            float angleDelta = 360f / amount;

            List<Vector2> positions = new List<Vector2>();

            while (amount > 0)
            {
                positions.Add(Utilities.CircleXY(m_radius, angle) + (Vector2)transform.position);
                angle += angleDelta;
                amount--;
            }

            return positions;
        }

        private void SpawnAcidField(Vector2 spawnPosition)
        {
            Transform acidFieldTransform = PoolBoss.SpawnInPool(m_weaponStats.GetWeaponProjectileParticle().transform, spawnPosition, Quaternion.identity);
            acidFieldTransform.GetComponent<AcidField>().StartAcidFieldSequence(
                m_weaponStats.GetWeaponProjectileParticle().transform.localScale,
                m_levelManager.playerController.fighter.Damage * m_projectileDamage,
                m_aoe * (1 + m_levelManager.playerController.fighter.weaponAoe),
                m_slowEffect,
                m_effectDuration * (1 + m_levelManager.playerController.fighter.weaponDuration));
        }

        public override void NormalLevelUp()
        {
            base.NormalLevelUp();
            UpdateVariables();
            SpawnAcidBottles();
        }

        public override void UltimateUpgrade()
        {
            base.UltimateUpgrade();
            UpdateVariables();
            m_projectileList.Clear();
            transform.Clear();
            SpawnAcidBottles();
        }
    }
}