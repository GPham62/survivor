using DarkTonic.PoolBoss;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WingsMob.Survival.Global;
using WingsMob.Survival.Model;

namespace WingsMob.Survival.Combat
{
    public class DrakeCaller : Weapon
    {
        [SerializeField] private BurnArea burnAreaPrefab;
        [SerializeField] private float m_fallHeight = 15f;
        [SerializeField] private Ease m_fallEase = Ease.InSine;
        private float m_damage, m_rechargeTime, m_speed, m_aoe, m_duration;
        private int m_projectileAmount;
        private bool m_isUltimate;

        public override void Init(Action<string> callback)
        {
            m_tweenAttackId = name;
            base.Init(callback);
            UpdateVariables();
            StartMeteorFallSequence();
            m_isUltimate = false;
        }

        public override void ResetStats()
        {
            DOTween.Kill(name, false);
            StartMeteorFallSequence();
        }

        private void UpdateVariables()
        {
            m_damage = m_weaponStats.GetWeaponParam(WeaponParam.Damage) / 100;
            m_rechargeTime = m_weaponStats.GetWeaponParam(WeaponParam.RechargeTime);
            m_projectileAmount = (int) m_weaponStats.GetWeaponParam(WeaponParam.ProjectileAmount);
            m_speed = m_weaponStats.GetWeaponParam(WeaponParam.ProjectileSpeed);
            m_aoe = m_weaponStats.GetWeaponParam(WeaponParam.AreaOfEffect);
            m_duration = m_weaponStats.GetWeaponParam(WeaponParam.Duration);
        }

        private void StartMeteorFallSequence()
        {
            Sequence meteorFallSequence = DOTween.Sequence();
            meteorFallSequence.AppendCallback(() =>
            {
                MeteorFall();
            })
                .AppendInterval(m_rechargeTime * (1 - m_levelManager.playerController.fighter.cooldownReduction))
                .SetId(name)
                .SetLoops(-1, LoopType.Restart);
        }

        private List<Vector2> m_fallPositions;
        private IDamageable m_tempEnemy;

        private void MeteorFall()
        {
            m_fallPositions = GenerateFallPositions(m_projectileAmount);
            for (int i = 0; i < m_fallPositions.Count; i++)
            {
                float height = UnityEngine.Random.Range(m_fallHeight - 0.5f, m_fallHeight + 0.5f);
                Meteor projectile = PoolBoss.SpawnInPool(m_weaponStats.GetWeaponProjectile().transform, new Vector2(m_fallPositions[i].x, m_fallPositions[i].y + height), Quaternion.identity).GetComponent<Meteor>();
                projectile.transform.gameObject.SetActive(true);
                projectile.Play();
                projectile.transform.DOMoveY(projectile.transform.position.y - height, height / m_speed).SetEase(m_fallEase).OnComplete(() =>
                {
                    Transform meteorImpact = PoolBoss.SpawnInPool(m_weaponStats.GetWeaponProjectileParticle().transform, projectile.transform.position, Quaternion.identity);
                    meteorImpact.gameObject.GetComponent<TimedDespawner>().LifeSeconds = m_duration * (1 + m_duration * m_levelManager.playerController.fighter.weaponDuration);
                    
                    var newAoe = m_aoe * (1 + m_levelManager.playerController.fighter.weaponAoe);
                    meteorImpact.localScale = Vector3.one * newAoe / 100;
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(meteorImpact.transform.position, newAoe / 200);
                    foreach (var col in colliders)
                    {
                        m_tempEnemy = col.GetComponent<IDamageable>();
                        if (col.isTrigger == true && m_tempEnemy != null && GameStatus.CurrentState == GameState.Playing)
                        {
                            m_tempEnemy.TakeDamage(m_levelManager.playerController.fighter.Damage * m_damage);
                        }
                    }
                    if (m_isUltimate)
                    {
                        BurnArea burnArea = PoolBoss.SpawnInPool(burnAreaPrefab.transform, projectile.transform.position, Quaternion.identity).GetComponent<BurnArea>();
                        burnArea.transform.localScale = Vector3.one * newAoe / 100;
                        burnArea.SetDamage(m_levelManager.playerController.fighter.Damage * m_damage / 10);
                        burnArea.SetInterval(0.1f);
                        burnArea.SetAoe(newAoe);
                        burnArea.TimedDespawn(m_duration * (1 + m_duration * m_levelManager.playerController.fighter.weaponDuration));
                    }

                    PoolBoss.Despawn(projectile.transform);
                });
            }
        }
        private List<Vector2> GenerateFallPositions(int fallAmount)
        {
            List<Vector2> fallPos = new List<Vector2>();
            fallPos = m_levelManager.enemyDetector.GetEnemiesPosOnScreen();
            if (fallPos.Count < fallAmount)
            {
                for (int i = fallPos.Count; i < fallAmount; i++)
                {
                    Vector2 randomPos = new Vector2(transform.position.x + UnityEngine.Random.Range(-5f, 5f), transform.position.y + UnityEngine.Random.Range(-5f, 5f));
                    if (Mathf.Abs(randomPos.x) < 0.1 && Mathf.Abs(randomPos.y) < 0.1)
                        randomPos = Vector2.one;
                    fallPos.Add(randomPos);
                }
                return fallPos;
            }
            else
                return fallPos.Take(fallAmount).ToList();
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
            m_isUltimate = true;
        }
    }
}