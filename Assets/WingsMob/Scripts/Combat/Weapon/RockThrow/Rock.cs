using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.PoolBoss;
using System;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Utils;
using WingsMob.Survival.Model;

namespace WingsMob.Survival.Combat
{
    public class Rock : WeaponProjectile
    {
        [SerializeField] private float m_rockLifeTime = 2f;

        private float m_speed;
        private float m_damage;
        private Vector2 m_direction;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other is Collider2D)
            {
                IDamageable enemy = other.GetComponent<IDamageable>();
                if (enemy != null)
                {
                    enemy.TakeDamage(m_damage);
                    if (m_projectileParticle)
                        PoolBoss.SpawnInPool(m_projectileParticle.transform, other.ClosestPoint(transform.position), m_projectileParticle.transform.rotation);
                    PoolBoss.Despawn(transform);
                }
            }
        }

        private Rigidbody2D m_rigidBody;
        private WeaponProjectileParticle m_projectileParticle;

        public void Init(Vector2 targetPos, float speed, float damage, WeaponProjectileParticle projectileParticle = null)
        {
            m_speed = speed;
            m_damage = damage;
            m_direction = targetPos - (Vector2)transform.position;
            m_projectileParticle = projectileParticle;
            float angle = Mathf.Atan2(m_direction.y, m_direction.x) * Mathf.Rad2Deg - 90;
            if (m_rigidBody == null)
                m_rigidBody = gameObject.GetComponent<Rigidbody2D>();

            m_rigidBody.rotation = angle;
            StartCoroutine(IEMoveWithDirection(m_direction));
            StartCoroutine(CoroutineUtils.DelayCallback(m_rockLifeTime, () =>
            {
                PoolBoss.Despawn(transform);
            }));
        }

        private float tempFlightTime;

        private IEnumerator IEMoveWithDirection(Vector2 direction)
        {
            tempFlightTime = m_rockLifeTime;
            while (tempFlightTime > 0)
            {
                m_rigidBody.velocity = direction.normalized * m_speed;
                tempFlightTime -= Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            m_rigidBody.velocity.Set(0f, 0f);
        }
    }
}
