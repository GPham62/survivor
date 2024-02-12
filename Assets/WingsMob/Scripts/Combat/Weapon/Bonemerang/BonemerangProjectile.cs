using DarkTonic.PoolBoss;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Global;
using WingsMob.Survival.Model;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Combat
{
    public class BonemerangProjectile : WeaponProjectile
    {
        [SerializeField] private Collider2D m_collider;
        [SerializeField] private List<SpriteRenderer> m_spriteComps;
        [SerializeField] private Rigidbody2D m_rigidBody;
        [SerializeField] private float m_angleSpeed = 150;
        private WeaponProjectileParticle m_projectileParticle;
        private float m_damage;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other is Collider2D)
            {
                IDamageable enemy = other.GetComponent<IDamageable>();
                if (enemy != null)
                {
                    if (GameStatus.CurrentState == GameState.Playing)
                        enemy.TakeDamage(m_damage);
                    if (m_projectileParticle)
                        PoolBoss.SpawnInPool(m_projectileParticle.transform, other.ClosestPoint(transform.position), m_projectileParticle.transform.rotation);
                }
            }
        }

        public void Init(Vector2 direction, float speed, float damage, float aoe, float range, WeaponProjectileParticle projectileParticle = null)
        {
            m_damage = damage;
            transform.localScale = new Vector2(aoe, aoe);
            m_projectileParticle = projectileParticle;
            Vector2 distanceToMove = direction.normalized * range / 100;
            Vector2 forwardPos = (Vector2)transform.position + distanceToMove;
            Vector2 backwardPos = (Vector2) transform.position - direction.normalized * range * 10 / 100;
            float returnTime = Vector2.Distance(transform.position, backwardPos) / (speed * 2f);

            m_rigidBody.DOMove(forwardPos, distanceToMove.magnitude / speed).SetEase(Ease.OutSine).OnComplete(
                () => m_rigidBody.DOMove(backwardPos, returnTime).SetEase(Ease.InSine).OnComplete(() => PoolBoss.Despawn(transform)));
        }

        public void InitUltimateSpiral(float speed, float damage, float duration, float aoe, float range, bool isLeft, WeaponProjectileParticle projectileParticle = null)
        {
            ResetVariables();
            m_damage = damage;
            transform.localScale = new Vector2(aoe, aoe);
            m_projectileParticle = projectileParticle;
            StartCoroutine(IESpiral(range/100, speed, isLeft));
            StartCoroutine(CoroutineUtils.DelayCallback(duration, () =>
            {
                PoolBoss.Despawn(transform);
            }));
        }

        private float m_angleMoved;
        private float m_spiralRadius;

        private void ResetVariables()
        {
            m_angleMoved = 0;
            m_spiralRadius = 0;
            var tempColor = m_spriteComps[0].color;
            tempColor.a = 1f;
            foreach (var sprite in m_spriteComps)
                sprite.color = tempColor;
            m_collider.enabled = true;
        }

        private IEnumerator IESpiral(float range, float speed, bool isLeft)
        {
            while (true)
            {
                if (Vector2.Distance(transform.position, LevelManager.Instance.playerController.mover.transform.position) >= range)
                {
                    foreach (var sprite in m_spriteComps)
                        sprite.DOFade(0, 0.3f).OnComplete(() => m_collider.enabled = false);
                }
                m_angleMoved += Time.fixedDeltaTime * m_angleSpeed;
                m_spiralRadius = isLeft ? m_spiralRadius + Time.fixedDeltaTime * speed : m_spiralRadius - Time.fixedDeltaTime * speed;
                float x = m_spiralRadius * Mathf.Cos(Mathf.Deg2Rad * m_angleMoved);
                float y = m_spiralRadius * Mathf.Sin(Mathf.Deg2Rad * m_angleMoved);
                transform.localPosition = new Vector2(x, y);
                yield return new WaitForFixedUpdate();
            }
        }
    }
}