using DarkTonic.PoolBoss;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Combat;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Global;

namespace WingsMob.Survival.Environment.Collectable
{
    public class Coin : MonoBehaviour, ICollectable
    {
        [Title("Tween Settings")]
        [SerializeField] private float distanceOut = 0.65f;
        [SerializeField] private Ease easeOut = Ease.Linear;
        [SerializeField] private Ease easeIn = Ease.Linear;
        [SerializeField] private float outTime = 0.21f;
        [SerializeField] private float inTime = 0.38f;

        [Title("Coin setting")]
        [SerializeField] private float m_coinAmount;

        private Collider2D m_collider;

        private Sequence m_seq;

        private LevelManager m_levelManager;

        private void Awake()
        {
            m_collider = GetComponent<Collider2D>();
        }

        public void OnCollected(Transform collector)
        {
            m_collider.enabled = false;

            transform.parent = collector;
            Ray ray = new Ray(transform.position, transform.position - collector.position);

            m_seq = DOTween.Sequence();
            m_seq.Append(transform.DOLocalMove(ray.GetPoint(distanceOut) - collector.position, outTime).SetEase(easeOut))
                .Append(transform.DOLocalMove(Vector3.zero, inTime).SetEase(easeIn))
                .AppendCallback(Collect)
                .SetAutoKill(true);
        }

        private void Collect()
        {
            GameAssets.Instance.CreateCoinsPopup(transform.position, (int) m_coinAmount);
            m_levelManager.gameStats.IncreaseGameStats(GamePlayStats.Coin, m_coinAmount);
            PoolBoss.Despawn(transform);
            //if (GameStatus.CurrentState == GameState.Playing)
            //    SoundManager.Instance.PlaySound(MusicNameDefine.GetCombatDiamondCollect());
        }

        public void SetReward(float amount)
        {
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
