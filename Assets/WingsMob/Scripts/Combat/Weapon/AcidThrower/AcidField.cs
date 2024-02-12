using DarkTonic.PoolBoss;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Global;
using WingsMob.Survival.Model;

namespace WingsMob.Survival.Combat
{
    public class AcidField : WeaponProjectileParticle
    {
        [SerializeField] private float m_scaleUpDuration = 0.25f;
        [SerializeField] private float m_scaleDownDuration = 0.25f;
        private bool isPause;
        private CircleCollider2D m_circleCollider;
        public void StartAcidFieldSequence(Vector2 originalScale, float damage, float aoe, float slowEffect, float effectDuration)
        {
            isPause = false;
            m_circleCollider = GetComponent<CircleCollider2D>();
            this.RegisterListener(MethodNameDefine.OnGamePaused, PauseAcidFieldRoutine);
            this.RegisterListener(MethodNameDefine.OnGameResumed, ResumeAcidFieldRoutine);
            StartCoroutine(IEAcidField(originalScale, damage, aoe, slowEffect, effectDuration));
        }

        private void PauseAcidFieldRoutine(object obj = null)
        {
            isPause = true;
        }

        private void ResumeAcidFieldRoutine(object obj = null)
        {
            isPause = false;
        }

        private Collider2D[] m_tempCollider;
        private IDamageable m_tempEnemy;

        private IEnumerator IEAcidField(Vector2 originalScale, float damage, float aoe, float slowEffect, float effectDuration)
        {
            transform.localScale = Vector2.zero;
            float tempTime = 0f;
            while  (tempTime < m_scaleUpDuration && !isPause)
            {
                transform.localScale = Vector2.Lerp(Vector2.zero, originalScale * aoe, tempTime / m_scaleUpDuration);
                tempTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            tempTime = 0f;
            while (tempTime < effectDuration && !isPause)
            {
                m_tempCollider = Physics2D.OverlapCircleAll(transform.position, m_circleCollider.radius * aoe);
                foreach (var col in m_tempCollider)
                {
                    m_tempEnemy = col.GetComponent<IDamageable>();
                    if (col.isTrigger == true && m_tempEnemy != null)
                    {
                        m_tempEnemy.TakeDamage(damage);
                    }
                }
                tempTime += 0.5f;
                yield return new WaitForSeconds(0.5f);
            }

            tempTime = 0f;
            while (tempTime < m_scaleDownDuration && !isPause)
            {
                transform.localScale = Vector2.Lerp(originalScale * aoe, Vector2.zero, tempTime / m_scaleUpDuration);
                tempTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            this.RemoveListener(MethodNameDefine.OnGamePaused, PauseAcidFieldRoutine);
            this.RemoveListener(MethodNameDefine.OnGameResumed, ResumeAcidFieldRoutine);
            PoolBoss.Despawn(transform);
        }

        private void OnDestroy()
        {
            this.RemoveListener(MethodNameDefine.OnGamePaused, PauseAcidFieldRoutine);
            this.RemoveListener(MethodNameDefine.OnGameResumed, ResumeAcidFieldRoutine);
        }

    }
}