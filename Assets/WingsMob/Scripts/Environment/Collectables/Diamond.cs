using UnityEngine;
using DarkTonic.PoolBoss;
using WingsMob.Survival.Combat;
using WingsMob.Survival.Global;
using DG.Tweening;
using WingsMob.Survival.Controller;
using System;

namespace WingsMob.Survival.Environment.Collectable
{
    public class Diamond : MonoBehaviour, ICollectable, IAttractable
    {
        [SerializeField] private float distanceOut = 0.65f;
        [SerializeField] private Ease easeOut = Ease.Linear;
        [SerializeField] private Ease easeIn = Ease.Linear;

        [SerializeField] private float outTime = 0.21f;

        [SerializeField] private float m_flightSpeed = 3.5f;

        private Collider2D m_collider;
        private float m_expReward;

        private Sequence m_seq;

        private LevelManager m_levelManager;

        private void Awake()
        {
            m_collider = GetComponent<Collider2D>();
        }

        public void SetReward(float exp)
        {
            m_expReward = exp;
        }

        public void OnAttracted(Transform attracter)
        {
            m_collider.enabled = false;

            transform.parent = attracter;
            Ray ray = new Ray(transform.position, transform.position - attracter.position);

            m_seq = DOTween.Sequence();
            m_seq.Append(transform.DOLocalMove(ray.GetPoint(distanceOut) - attracter.position, outTime).SetEase(easeOut))
                .Append(transform.DOLocalMove(Vector3.zero, Vector2.Distance(transform.position, attracter.position) / (m_flightSpeed*2)).SetEase(easeIn))
                .AppendCallback(Collect)
                .SetAutoKill(true);
        }

        public void OnCollected(Transform collector)
        {
            m_collider.enabled = false;

            transform.parent = collector;
            Ray ray = new Ray(transform.position, transform.position - collector.position);

            m_seq = DOTween.Sequence();
            m_seq.Append(transform.DOLocalMove(ray.GetPoint(distanceOut) - collector.position, outTime).SetEase(easeOut))
                .Append(transform.DOLocalMove(Vector3.zero, Vector2.Distance(transform.position, collector.position) / (m_flightSpeed * 2)).SetEase(easeIn))
                .AppendCallback(Collect)
                .SetAutoKill(true);
        }

        private void Collect()
        {
            PoolBoss.Despawn(transform);
            if (GameStatus.CurrentState != GameState.Playing)
                return;
            //SoundManager.Instance.PlaySound(MusicNameDefine.GetCombatDiamondCollect());
            m_levelManager.gameStats.IncreaseGameStats(GamePlayStats.Exp, m_expReward);

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