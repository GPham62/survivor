using DarkTonic.PoolBoss;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Global;
using WingsMob.Survival.Model;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Combat
{
    public class BouncingBullet : WeaponProjectile
    {
        private float m_speed, m_damage;
        private int m_bounceCountMax, m_bounceCount;
        private Rigidbody2D m_rigidBody;
        private WeaponProjectileParticle m_projectileParticle;
        private Vector3 m_lastVelocity;
        private bool m_isSplittable;
        private Transform m_splitTarget;
        private Action<Transform> m_despawnCallback;
        private Action<Transform> m_ultimateCallback;

        private void Awake()
        {
            m_rigidBody = gameObject.GetComponent<Rigidbody2D>();
            m_rigidBody.angularVelocity = 1000;
        }

        private void OnEnable()
        {
            m_bounceCount = 0;
        }

        public void Init(Vector2 direction, float speed, float damage, float aoe, int bounceCount, bool isSplittable, Transform splitTarget = null, WeaponProjectileParticle projectileParticle = null, Action<Transform> onSpawnCallback = null, Action<Transform> onDespawnCallback = null)
        {
            m_speed = speed;
            m_damage = damage;
            transform.localScale = new Vector3(aoe, aoe, aoe);
            m_bounceCountMax = bounceCount;
            m_isSplittable = isSplittable;
            m_splitTarget = splitTarget;
            m_projectileParticle = projectileParticle;
            m_ultimateCallback = onSpawnCallback;
            m_despawnCallback = onDespawnCallback;
            ChangeRotation(direction);
            m_rigidBody.velocity = direction.normalized * m_speed;
        }

        private void ChangeRotation(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            m_rigidBody.rotation = angle;
        }


        private void FixedUpdate()
        {
            m_lastVelocity = m_rigidBody.velocity;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            m_bounceCount++;
            if (m_projectileParticle)
                PoolBoss.SpawnInPool(m_projectileParticle.transform, collision.contacts[0].point, m_projectileParticle.transform.rotation);
            Vector2 reflectDirection = Vector2.Reflect(m_lastVelocity, collision.contacts[0].normal);
            ChangeRotation(reflectDirection);

            if (reflectDirection == Vector2.zero)
                reflectDirection = Vector2.up;

            m_rigidBody.velocity = reflectDirection.normalized * m_speed;


            if (collision.gameObject.CompareTag(SurvivorConfig.PlayerTag))
                return;
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                if (GameStatus.CurrentState == GameState.Playing)
                    damageable.TakeDamage(m_damage);
                if (m_isSplittable)
                {
                    m_isSplittable = false;
                    float angle = 45f;
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 direction = Quaternion.AngleAxis(angle, Vector3.forward) * reflectDirection;
                        Transform newBulletTransform = PoolBoss.SpawnInPool(m_splitTarget, new Vector2(transform.position.x + direction.normalized.x, transform.position.y + direction.normalized.y), Quaternion.identity);
                        newBulletTransform.GetComponent<BouncingBullet>().Init(
                            direction, m_speed, m_damage, transform.localScale.x, m_bounceCountMax, false, null, m_projectileParticle, null, m_despawnCallback);
                        m_ultimateCallback?.Invoke(newBulletTransform);
                        angle += 270f;
                    }
                }
            }

            if (m_bounceCount >= m_bounceCountMax)
            {
                m_despawnCallback?.Invoke(transform);
                PoolBoss.Despawn(transform);
            }
        }
    }
}
