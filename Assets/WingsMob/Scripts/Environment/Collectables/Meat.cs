using UnityEngine;
using DarkTonic.PoolBoss;
using WingsMob.Survival.Combat;
using WingsMob.Survival.Global;
using DG.Tweening;
using WingsMob.Survival.Controller;

namespace WingsMob.Survival.Environment.Collectable
{
    public class Meat : MonoBehaviour, ICollectable
    {
        [SerializeField] private float distanceOut = 0.65f;
        [SerializeField] private Ease easeOut = Ease.Linear;
        [SerializeField] private Ease easeIn = Ease.Linear;

        [SerializeField] private float outTime = 0.21f;
        [SerializeField] private float inTime = 0.38f;

        private Collider2D m_collider;
        private float m_healthBonus;

        private Sequence m_seq;

        private LevelManager m_gameManager;

        private void Awake()
        {
            m_collider = GetComponent<Collider2D>();
        }

        public void SetReward(float amount)
        {
            m_healthBonus = amount;
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
            //SoundManager.Instance.PlaySound(MusicNameDefine.GetCombatHeal());
            LevelManager.Instance.playerController.fighter.GainHealth(m_healthBonus);
            PoolBoss.Despawn(transform);
            if (GameStatus.CurrentState == GameState.Playing)
                GameAssets.Instance.CreateHealPopup(transform.position, (int)m_healthBonus);
            
        }

        private void OnEnable()
        {
            if (m_gameManager == null)
                m_gameManager = LevelManager.Instance;
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