using DarkTonic.PoolBoss;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Combat;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Global;

namespace WingsMob.Survival.Environment.Collectable
{
    public class Bomb : MonoBehaviour, ICollectable
    {
        [SerializeField] private float m_bombRadius = 7.5f;

        [SerializeField] private float distanceOut = 0.65f;
        [SerializeField] private Ease easeOut = Ease.Linear;
        [SerializeField] private Ease easeIn = Ease.Linear;

        [SerializeField] private float outTime = 0.21f;

        [SerializeField] private float m_flightSpeed = 3.5f;

        private Collider2D m_collider;

        private Sequence m_seq;

        private LevelManager m_levelManager;

        private void Awake()
        {
            m_collider = GetComponent<Collider2D>();
        }

        public void SetReward(float amount)
        {
            throw new System.NotImplementedException();
        }

        public void OnCollected(Transform collector)
        {
            m_collider.enabled = false;

            transform.parent = collector;
            Ray ray = new Ray(transform.position, transform.position - collector.position);

            m_seq = DOTween.Sequence();
            m_seq.Append(transform.DOLocalMove(ray.GetPoint(distanceOut) - collector.position, outTime).SetEase(easeOut))
                .Append(transform.DOLocalMove(Vector3.zero, Vector2.Distance(transform.position, collector.position) / m_flightSpeed).SetEase(easeIn))
                .AppendCallback(() => Collect(collector))
                .SetAutoKill(true);
        }

        private void Collect(Transform collector)
        {
            PoolBoss.Despawn(transform);
            if (GameStatus.CurrentState != GameState.Playing)
                return;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(collector.position, m_bombRadius);
            foreach (var col in colliders)
            {
                if (col.CompareTag(SurvivorConfig.EnemyTag))
                {
                    var damageable = col.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeLethalDamage();
                    }
                }
            }
        }

        private void OnEnable()
        {
            if (m_levelManager == null)
                m_levelManager = LevelManager.Instance;
            m_collider.enabled = true;
            this.RegisterListener(MethodNameDefine.OnGamePaused, PauseCollectSequence);
            this.RegisterListener(MethodNameDefine.OnGameResumed, ResumeCollectSequence);
        }

        private void PauseCollectSequence(object obj)
        {
            m_seq.Pause();
        }

        private void ResumeCollectSequence(object obj)
        {
            m_seq.TogglePause();
        }

        private void OnDisable()
        {
            this.RemoveListener(MethodNameDefine.OnGamePaused, PauseCollectSequence);
            this.RemoveListener(MethodNameDefine.OnGameResumed, ResumeCollectSequence);
        }
    }
}