using DarkTonic.PoolBoss;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Combat;
using WingsMob.Survival.Controller;
using WingsMob.Survival.GameConfig;
using WingsMob.Survival.Global;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Environment.Collectable
{
    public class SupplyBox : MonoBehaviour, ICollectable, IDamageable
    {
        [SerializeField] SupplyBoxChanceConfig supplyBoxConfig;

        private LevelManager m_levelManager;
        private Collider2D m_collider;
        private Animator m_animator;
        private int animationOpenHash = Animator.StringToHash("Open");
        private int animationIdleHash = Animator.StringToHash("Idle");

        private void Awake()
        {
            m_collider = GetComponent<Collider2D>();
            m_animator = GetComponent<Animator>();
        }

        public void OnCollected(Transform collector)
        {
            TriggerBox();
        }

        private void TriggerBox()
        {
            m_collider.enabled = false;
            m_animator.Play(animationOpenHash);
            StartCoroutine(CoroutineUtils.DelayCallback(0.3f, () => {
                RandomizeSupply();
                PoolBoss.Despawn(transform);
            }));
        }

        private void RandomizeSupply()
        {
            float rand = Random.Range(0, 100f);
            _ = rand <= supplyBoxConfig.chanceConfig.coinChance ? m_levelManager.collectableSpawner.SpawnCoin(transform.position)
                : rand <= supplyBoxConfig.chanceConfig.coinChance + supplyBoxConfig.chanceConfig.coinPackChance ? m_levelManager.collectableSpawner.SpawnCoinPack(transform.position)
                : rand <= supplyBoxConfig.chanceConfig.coinChance + supplyBoxConfig.chanceConfig.coinPackChance + supplyBoxConfig.chanceConfig.meatChance ? m_levelManager.collectableSpawner.SpawnMeat(transform.position)
                : rand <= supplyBoxConfig.chanceConfig.coinChance + supplyBoxConfig.chanceConfig.coinPackChance + supplyBoxConfig.chanceConfig.meatChance + supplyBoxConfig.chanceConfig.magnetChance ? m_levelManager.collectableSpawner.SpawnMagnet(transform.position)
                : m_levelManager.collectableSpawner.SpawnBomb(transform.position);
        }

        public void SetReward(float amount)
        {
        }

        public void TakeDamage(float damage)
        {
            TriggerBox();
        }

        public void KnockBack(Vector2 direction, float strength, float duration)
        {
        }

        private void OnEnable()
        {
            if (m_levelManager == null)
                m_levelManager = LevelManager.Instance;
            m_collider.enabled = true;
            m_animator.Play(animationIdleHash);
        }

        public void TakeLethalDamage()
        {
        }
    }
}
