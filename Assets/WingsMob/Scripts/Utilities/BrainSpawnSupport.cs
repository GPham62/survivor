using DarkTonic.PoolBoss;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Boss.Skill;
using WingsMob.Survival.Combat;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Global;
using WingsMob.Survival.Stats;

namespace WingsMob.Survival.Utils
{
    public class BrainSpawnSupport : MonoBehaviour
    {
        [SerializeField] BaseStats stats;
        [SerializeField] Transform minionPrefab;
        [SerializeField] Transform bombPrefab;
        [SerializeField] private Transform m_bombExplosionEffect;

        public void SpawnMinions()
        {
            LevelManager levelManager = LevelManager.Instance;
            for (int i = 0; i < stats.GetBaseStat(Global.CharacterStats.MinionNum); i++)
            {
                if (levelManager.brainCenter.IsFull()) return;
                Transform m_enemyTransform = PoolBoss.SpawnInPool(minionPrefab, transform.position, transform.rotation);
                EnemyController m_enemyControllerTemp = m_enemyTransform.GetComponent<EnemyController>();
                //m_enemyControllerTemp.ChangeStats(stats.GetLevel());
                levelManager.brainCenter.AddEnemyToArr(m_enemyControllerTemp);
            }
        }

        public void SpawnBomb()
        {
            //MolestBomb bomb = PoolBoss.SpawnInPool(bombPrefab, transform.position, transform.rotation).GetComponent<MolestBomb>();
            //bomb.SetDamage(stats.GetBaseStat(Global.CharacterStats.AttackBase));
            //bomb.TriggerExplosion();
            Transform explosion = PoolBoss.SpawnInPool(m_bombExplosionEffect, transform.position, m_bombExplosionEffect.rotation);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1.2f * explosion.localScale.x);
            foreach (var col in colliders)
            {
                if (col.CompareTag(SurvivorConfig.PlayerTag))
                {
                    var playerFighter = col.GetComponentInParent<PlayerFighter>();
                    if (playerFighter != null)
                    {
                        playerFighter.TakeDamage(stats.GetBaseStat(CharacterStats.AttackBase));
                        return;
                    }
                }
            }
        }
    }
}
