using DarkTonic.PoolBoss;
using DG.Tweening;
using Sirenix.OdinInspector;
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
    public class BananadeProjectile : WeaponProjectile
    {
        [SerializeField] float minDistance = 3f;
        [SerializeField] float timeFriction = 10f;
        [Title("Bounce 1")]
        [SerializeField] float heightFriction1;
        [SerializeField] float distanceFriction1;

        [Title("Bounce 2")]
        [SerializeField] float heightFriction2;
        [SerializeField] float distanceFriction2;

        [Title("Bounce 3")]
        [SerializeField] float heightFriction3;
        private WeaponProjectileParticle m_projectileParticle;
        private Collider2D[] m_tempCollider;
        private IDamageable m_tempEnemy;

        private Tween m_moveTween;
        private float m_damage, m_aoe;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other is Collider2D)
            {
                IDamageable enemy = other.GetComponent<IDamageable>();
                if (enemy != null)
                {
                    m_moveTween.Kill(false);
                    DealAoeDamage(m_damage, m_aoe);
                    PoolBoss.Despawn(transform);
                }
            }
        }

        public void Init(Vector2 targetPos, float speed, float damage, float aoe, WeaponProjectileParticle projectileParticle = null)
        {
            m_damage = damage;
            m_aoe = aoe;
            m_projectileParticle = projectileParticle;
            m_moveTween = transform.DOMove(targetPos, Vector2.Distance(targetPos, transform.position) / speed).SetEase(Ease.Linear).OnComplete(() => {
                DealAoeDamage(damage, aoe);
                PoolBoss.Despawn(transform);
            });
        }

        private void DealAoeDamage(float damage, float aoe)
        {

            m_tempCollider = Physics2D.OverlapCircleAll(transform.position, aoe / 2);
            foreach (var col in m_tempCollider)
            {
                m_tempEnemy = col.GetComponent<IDamageable>();
                if (col.isTrigger == true && m_tempEnemy != null)
                {
                    if (GameStatus.CurrentState == GameState.Playing)
                        m_tempEnemy.TakeDamage(damage);
                }
            }

            if (m_projectileParticle)
            {
                Transform particleTransform = PoolBoss.SpawnInPool(m_projectileParticle.transform, transform.position, m_projectileParticle.transform.rotation);
                particleTransform.localScale = Vector3.one * aoe / 2;
            }
        }

        public void InitUltimate(Vector2 targetPos, float speed, float damage, float aoe, WeaponProjectileParticle projectileParticle = null)
        {
            m_projectileParticle = projectileParticle;
            
            StartCoroutine(BounceCoroutine(damage, targetPos, speed, () => DealAoeDamage(damage, aoe)));
        }

        private IEnumerator BounceCoroutine(float damage, Vector2 targetPos, float speed, Action bounceFinishCallBack)
        {
            Vector2 bouncePos1 = Utilities.GetPointWithFriction(transform.position, targetPos, distanceFriction1);
            yield return BezierCurveToTarget(transform.position, bouncePos1, speed, heightFriction1, bounceFinishCallBack);
            Vector2 bouncePos2 = Utilities.GetPointWithFriction(bouncePos1, targetPos, distanceFriction2);
            yield return BezierCurveToTarget(bouncePos1, bouncePos2, speed, heightFriction2, bounceFinishCallBack);
            yield return BezierCurveToTarget(bouncePos2, targetPos, speed, heightFriction3, bounceFinishCallBack);
            PoolBoss.Despawn(transform);
            
        }

        private IEnumerator BezierCurveToTarget(Vector2 startPos, Vector2 endPos, float speed, float heightFriction, Action callback)
        {
            Vector2 bezierPos = CalculateBezierPoint(startPos, endPos, heightFriction);
            float elapsedTime = Mathf.Min(Vector2.Distance(endPos, startPos), 1f);
            while (elapsedTime > 0)
            {
                Vector3 i1 = Vector3.Lerp(endPos, bezierPos, elapsedTime);
                Vector3 i2 = Vector3.Lerp(bezierPos, startPos, elapsedTime);
                transform.position = Vector3.Lerp(i1, i2, elapsedTime);
                elapsedTime -= Time.deltaTime * speed;
                yield return null;
            }
            callback.Invoke();
        }

        private Vector3 CalculateBezierPoint(Vector2 pos1, Vector2 pos2, float friction)
        {
            Vector2 middlePointPos = Utilities.GetMiddlePoint(pos1, pos2);
            float distance = Vector2.Distance(pos1, pos2) / 2 * friction;
            Vector2 result = middlePointPos + (pos1.x > pos2.x ? 1 : -1) * (Vector2.Perpendicular(pos1 - pos2).normalized * distance);
            return result;
        }


    }
}

