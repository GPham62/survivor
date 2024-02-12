using DarkTonic.PoolBoss;
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
    public class LuciferProjectile : WeaponProjectile
    {
        [SerializeField] private float m_maxHeightFriction = 1.5f;
        [SerializeField] private float m_maxDistanceFriction = 0.2f;
        [SerializeField] private Rigidbody2D m_rigidBody;

        private WeaponProjectileParticle m_projectileParticle;

        public enum FlyDirection
        {
            Up,
            Straight,
            Down
        }
        public void Fly(FlyDirection direction, Vector2 target, float damage, float aoe, float speed, float bulletPosFriction, bool isReverse = false, WeaponProjectileParticle projectileParticle = null)
        {
            m_projectileParticle = projectileParticle;
            switch (direction)
            {
                case FlyDirection.Up:
                    FlyUp(target, speed, bulletPosFriction, () => DealAoeDamage(damage, aoe), isReverse);
                    break;
                case FlyDirection.Straight:
                    FlyStraight(target, speed, () => DealAoeDamage(damage, aoe));
                    break;
                case FlyDirection.Down:
                    FlyDown(target, speed, bulletPosFriction, () => DealAoeDamage(damage, aoe), isReverse);
                    break;
            }
        }

        private Collider2D[] m_tempCollider;
        private IDamageable m_tempEnemy;

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

        public void FlyUp(Vector2 target, float speed, float bulletPosFriction, Action callback, bool isReverse = false)
        {
            float heightFriction = bulletPosFriction * m_maxHeightFriction;
            float distanceFriction = (1 - bulletPosFriction) * m_maxDistanceFriction;
            StartCoroutine(BezierCurveToTarget(transform.localPosition, new Vector2(target.x + UnityEngine.Random.Range(-1f, 1f), target.y), speed, heightFriction, distanceFriction, callback, isReverse));
        }

        public void FlyStraight(Vector2 target, float speed, Action callback)
        {
            StartCoroutine(BezierCurveToTarget(transform.localPosition, target, speed, 0, 0.1f, callback));
        }

        public void FlyDown(Vector2 target, float speed, float bulletPosFriction, Action callback, bool isReverse = false)
        {
            float heightFriction = (1 - bulletPosFriction) * m_maxHeightFriction; 
            float distanceFriction = bulletPosFriction * m_maxDistanceFriction;
            StartCoroutine(BezierCurveToTarget(transform.localPosition, new Vector2(target.x + UnityEngine.Random.Range(-1f, 1f), target.y), speed, heightFriction, distanceFriction, callback, !isReverse));
        }

        private IEnumerator BezierCurveToTarget(Vector2 startPos, Vector2 endPos, float speed, float heightFriction, float distanceFriction, Action callback, bool isReverse = false)
        {
            Vector2 bezierPos = CalculateBezierPoint(startPos, endPos, heightFriction, distanceFriction, isReverse);
            float elapsedTime = Mathf.Min(Vector2.Distance(endPos, startPos), 1f);
            while (elapsedTime > 0)
            {
                Vector3 i1 = Vector3.Lerp(endPos, bezierPos, elapsedTime);
                Vector3 i2 = Vector3.Lerp(bezierPos, startPos, elapsedTime);
                Vector3 i = Vector3.Lerp(i1, i2, elapsedTime);
                var flyDirection = LevelManager.Instance.playerController.mover.transform.TransformDirection(i - transform.localPosition).normalized;
                Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, flyDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, speed * 10);
                transform.localPosition = i;
                elapsedTime -= Time.deltaTime * speed;
                yield return null;
            }
            callback.Invoke();
            PoolBoss.Despawn(transform);
        }

        private Vector3 CalculateBezierPoint(Vector2 pos1, Vector2 pos2, float heightFriction, float distanceFriction, bool isReverse = false)
        {
            Vector2 pointBetweenPos = Utilities.GetPointWithFriction(pos1, pos2, distanceFriction);
            float distance = Vector2.Distance(pos1, pos2) / 2 * heightFriction;
            Vector2 bezierPointOffset = (pos1.x > pos2.x ? 1 : -1) * (Vector2.Perpendicular(pos1 - pos2).normalized * distance);
            if (isReverse)
                bezierPointOffset = -bezierPointOffset;
            Vector2 result = pointBetweenPos + bezierPointOffset;
            return result;
        }
    }
}
