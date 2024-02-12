using DarkTonic.PoolBoss;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Global;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Combat
{
    public class BurnArea : MonoBehaviour
    {
        [SerializeField] CircleCollider2D m_circleCollider;
        private float m_interval;
        private float m_cd, m_damage;
        private float m_aoe;
        private bool m_isStop;

        private void Update()
        {
            if (GameStatus.CurrentState != GameState.Playing)
                return;

            m_cd += Time.deltaTime;
            if (m_cd > m_interval)
            {
                m_cd = 0;
                DealAreaDamage();
            }
        }

        private void DealAreaDamage()
        {
            if (m_isStop)
                return;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, m_circleCollider.radius * m_aoe / 100);
            foreach (var col in colliders)
            {
                IDamageable enemy = col.GetComponent<IDamageable>();
                if (col.isTrigger == true && enemy != null)
                {
                    enemy.TakeDamage(LevelManager.Instance.playerController.fighter.Damage * m_damage / 20);
                }
            }
        }

        private void OnEnable()
        {
            m_cd = 0;
            m_isStop = false;
        }

        public void SetDamage(float damage) => m_damage = damage;

        public void SetInterval(float interval) => m_interval = interval;

        public void SetAoe(float aoe) => m_aoe = aoe;

        public void TimedDespawn(float time)
        {
            StartCoroutine(CoroutineUtils.DelayCallback(time, () => {
                m_isStop = true;
                transform.DOScale(0f, 0.3f).OnComplete(() => PoolBoss.Despawn(transform));
            }));
        }
    }
}
