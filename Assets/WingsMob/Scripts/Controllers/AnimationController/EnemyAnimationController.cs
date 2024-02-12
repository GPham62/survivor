using DarkTonic.PoolBoss;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Combat;
using WingsMob.Survival.Global;
using WingsMob.Survival.Stats;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Controller
{
    public class EnemyAnimationController : MonoBehaviour, IAnimationController
    {
        [SerializeField] private SpriteRenderer m_shadowSprite;
        private SpriteRenderer m_sprite;
        private Animator m_animator;
        public void Init()
        {
            m_sprite = GetComponent<SpriteRenderer>();
            m_animator = GetComponent<Animator>();
        }

        public void EnableSprite()
        {
            m_sprite.enabled = true;
            m_shadowSprite.enabled = true;
        }

        public void DisableSprite()
        {
            m_sprite.enabled = false;
            m_shadowSprite.enabled = false;
        }

        public void Reset()
        {
            EnableSprite();
        }

        public void Idle()
        {
        }

        public void Run()
        {
        }

        public void StopAnimator() => m_animator.speed = 0;

        public void ResumeAnimator() => m_animator.speed = 1;

        public void Death(Transform caller, Action callback = null, bool isPoolBossObj = true)
        {
            DisableSprite();
            Transform newDeadEffect = PoolBoss.Spawn(GameAssets.Instance.enemyDeadEffect, new Vector3(transform.parent.position.x, transform.parent.position.y, transform.parent.position.z), transform.parent.rotation, transform.parent);

            StartCoroutine(CoroutineUtils.DelayCallback(0.7f, () =>
            {
                callback?.Invoke();
                PoolBoss.Despawn(newDeadEffect);
                if (isPoolBossObj)
                    PoolBoss.Despawn(caller);
                else
                    Destroy(caller.gameObject);
            }));
        }
    }
}