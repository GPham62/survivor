using DarkTonic.PoolBoss;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Combat;
using WingsMob.Survival.Global;

namespace WingsMob.Survival.Boss.Skill
{
    public class MolestBomb : MonoBehaviour
    {
        [SerializeField] SpriteRenderer sprite;
        [SerializeField] private float bombTickDuration = 2f;
        [SerializeField] private Transform m_explosionBefore;
        [SerializeField] private Transform m_explosionAfter;
        [SerializeField] private CircleCollider2D m_circleCollider;
        [SerializeField] private Transform m_bombExplosionEffect;
        private float m_bombDamage;

        private void OnDisable()
        {
            sprite.enabled = false;
            m_explosionBefore.gameObject.SetActive(false);
            m_explosionAfter.localScale = Vector2.zero;
        }

        private void OnEnable()
        {
            sprite.enabled = true;
        }

        public void DealDamageToPlayer()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, m_circleCollider.radius);
            foreach (var col in colliders)
            {
                if (col.CompareTag(SurvivorConfig.PlayerTag))
                {
                    var playerFighter = col.GetComponentInParent<PlayerFighter>();
                    if (playerFighter != null)
                    {
                        playerFighter.TakeDamage(m_bombDamage);
                        return;
                    }
                }
            }
        }

        public void SetDamage(float damage)
        {
            m_bombDamage = damage;
        }

        public void TriggerExplosion()
        {
            m_explosionBefore.gameObject.SetActive(true);
            m_explosionAfter.DOScale(m_explosionBefore.localScale, bombTickDuration).OnComplete(() =>
            {
                PoolBoss.SpawnInPool(m_bombExplosionEffect, transform.position, m_bombExplosionEffect.rotation);
                DealDamageToPlayer();
                PoolBoss.Despawn(transform);
            });
        }
    }
}