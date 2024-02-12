using DarkTonic.PoolBoss;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Global;
using WingsMob.Survival.Model;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Combat
{
    public class Icicle : WeaponProjectile
    {
        [SerializeField] private float m_pushStrength;
        [SerializeField] private float m_pushDuration;
        private float m_speed;
        private float m_damage;
        private float m_duration;
        private Rigidbody2D m_rigidBody;
        private WeaponProjectileParticle m_projectileParticle;
        //private bool m_isKnockBack;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other is Collider2D)
            {
                IDamageable enemy = other.GetComponent<IDamageable>();
                if (enemy != null)
                {
                    if (GameStatus.CurrentState == GameState.Playing)
                        enemy.TakeDamage(m_damage);
                    enemy.KnockBack((other.gameObject.transform.position - transform.position).normalized, m_pushStrength, m_pushDuration);

                    if (m_projectileParticle)
                        PoolBoss.SpawnInPool(m_projectileParticle.transform, other.ClosestPoint(transform.position), m_projectileParticle.transform.rotation);
                }
            }
        }

        public void Init(Vector2 direction, float speed, float damage, float duration, WeaponProjectileParticle projectileParticle = null)
        {
            m_speed = speed;
            m_damage = damage;
            m_duration = duration;
            m_projectileParticle = projectileParticle;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            if (m_rigidBody == null)
                m_rigidBody = gameObject.GetComponent<Rigidbody2D>();

            m_rigidBody.rotation = angle;
            Vector2 endPos = (Vector2)transform.position + direction.normalized * 40;
            Tween moveTween = transform.DOMove(endPos, Vector2.Distance(endPos, transform.position) / speed).SetEase(Ease.Linear);
            StartCoroutine(CoroutineUtils.DelayCallback(m_duration, () =>
            {
                moveTween.Kill(false);
                PoolBoss.Despawn(transform);
            }));
        }
    }
}
