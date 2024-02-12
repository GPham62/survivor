using Sirenix.OdinInspector;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using WingsMob.Survival.Combat;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Global;
using WingsMob.Survival.Stats;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Model
{
    [RequireComponent(typeof(BaseStats))]
    public class SurvivorBoss : MonoBehaviour, IDamageable, IAttacker
    {
        [Title("Boss Ref")]
        public string bossName;
        [SerializeField] protected GameObject m_skin;
        [SerializeField] protected GameObject m_bossCage;
        [SerializeField] private GameObject m_spawnParticle;
        [SerializeField] private Vector2 m_spawnOffset;
        [SerializeField] private GameObject m_afterSpawnParticle;
        [SerializeField] private Vector2 m_afterSpawnOffset;
        [SerializeField] private Image m_bossHealthImg;
        [SerializeField] private bool m_isBossPositionRandom;
        [DisableIf("m_isBossPositionRandom")] [SerializeField] private Vector2 m_bossSpawnPosOffset;
        [DisableIf("m_isBossPositionRandom")] [SerializeField] public bool isBossSpawnCenterMap = false;

        [Title("Other")]
        [SerializeField] private GameObject m_playerPos;

        [TitleGroup("Animations")]
        [SpineAnimation] [SerializeField] protected string m_idleAnimation;
        [TitleGroup("Animations")]
        [SpineAnimation] [SerializeField] private string m_dieAnimation;

        protected BaseStats m_stats;
        protected Transform m_player;
        protected Collider2D m_collider;
        protected float m_maxHP, m_damage, m_speed;
        private float m_currentHP;
        protected SkeletonAnimation m_skeleton;
        protected bool m_isLookingAtPlayer;

        private void OnEnable()
        {
            this.RegisterListener(MethodNameDefine.OnGamePaused, PauseTime);
            this.RegisterListener(MethodNameDefine.OnGameResumed, ResumeTime);
            this.RegisterListener(MethodNameDefine.OnGameOver, OnGameOver);
        }

        protected virtual void OnGameOver(object obj)
        {
            PauseTime(obj);
        }

        protected virtual void PauseTime(object obj)
        {
        }

        protected virtual void ResumeTime(object obj)
        {
        }

        private void OnDisable()
        {
            this.RemoveListener(MethodNameDefine.OnGamePaused, PauseTime);
            this.RemoveListener(MethodNameDefine.OnGameResumed, ResumeTime);
            this.RemoveListener(MethodNameDefine.OnGameOver, OnGameOver);
        }

        public virtual void Init()
        {
            m_stats = GetComponent<BaseStats>();
            m_collider = GetComponent<Collider2D>();
            m_skeleton = m_skin.GetComponent<SkeletonAnimation>();
            m_stats.SetLevel(1);
            m_isLookingAtPlayer = false;
            m_player = LevelManager.Instance.playerController.mover.transform;
            m_bossHealthImg.fillAmount = 1;
            int level = LevelManager.Instance.levelId + 1;
            int waveCount = LevelManager.Instance.scenarioHandler.waveCount;
            m_maxHP = (m_stats.GetBaseStat(CharacterStats.HealthBase) + m_stats.GetBaseStat(CharacterStats.HealthAddUp) * waveCount * SurvivorConfig.healthFactor) * Mathf.Pow(1.35f, level);
            m_damage = (m_stats.GetBaseStat(CharacterStats.AttackBase) + m_stats.GetBaseStat(CharacterStats.AttackAddUp) * waveCount * SurvivorConfig.attackFactor) * Mathf.Pow(1.42f, level);
            m_currentHP = m_maxHP;
        }

        protected GameObject m_bossCageRef;

        public void SpawnBossCage()
        {
            Hide();
            if (isBossSpawnCenterMap)
            {
                transform.position = new Vector2(0, m_player.position.y + m_bossSpawnPosOffset.y);
                Instantiate(m_spawnParticle, new Vector2(transform.position.x - m_spawnOffset.x, transform.position.y - m_spawnOffset.y), Quaternion.identity);
                m_bossCageRef = Instantiate(m_bossCage, new Vector2(0, m_player.position.y), Quaternion.identity);
            }
            else
            {
                transform.position = m_isBossPositionRandom ?
                new Vector2(m_player.position.x + Random.Range(-3, 3), m_player.position.y + Random.Range(-3, 3))
                : new Vector2(m_player.position.x + m_bossSpawnPosOffset.x, m_player.position.y + m_bossSpawnPosOffset.y);
                Instantiate(m_spawnParticle, new Vector2(transform.position.x - m_spawnOffset.x, transform.position.y - m_spawnOffset.y), Quaternion.identity);
                m_bossCageRef = Instantiate(m_bossCage, m_player.position, Quaternion.identity);
            }
        }

        public void Hide()
        {
            m_collider.enabled = false;
            m_skin.SetActive(false);
        }

        public void Appear()
        {
            m_skin.SetActive(true);
            Instantiate(m_afterSpawnParticle, new Vector2(transform.position.x - m_afterSpawnOffset.x, transform.position.y - m_afterSpawnOffset.y), Quaternion.identity);
            m_collider.enabled = true;
        }

        public virtual void TakeDamage(float damage)
        {
            if (!IsAlive()) return;
            m_currentHP = m_currentHP - damage < 0 ? 0 : m_currentHP - damage;
            m_bossHealthImg.fillAmount = m_currentHP / m_maxHP;
            GameAssets.Instance.CreateDamagePopup(transform.position, (int)damage);
            if (m_currentHP <= 0)
            {
                Die();
            }
        }

        public void KnockBack(Vector2 direction, float strength, float duration)
        {
        }

        public bool IsAlive()
        {
            return m_currentHP > 0;
        }

        public virtual void Die()
        {
            Destroy(m_bossCageRef);
            m_isLookingAtPlayer = false;
            m_skeleton.AnimationState.SetAnimation(0, m_dieAnimation, false);
            StartCoroutine(CoroutineUtils.DelayCallback(m_skeleton.Skeleton.Data.FindAnimation(m_dieAnimation).Duration + 0.2f, () =>
            {
                if (!LevelManager.Instance.scenarioHandler.IsLastScenario())
                {
                    LevelManager.Instance.collectableSpawner.SpawnWeaponChest(transform.position, Random.Range(0, 100));
                    LevelManager.Instance.collectableSpawner.SpawnMeat(transform.position);
                    Destroy(gameObject);
                }
            }));
            this.PostEvent(MethodNameDefine.OnBossFightEnd);
            this.PostEvent(MethodNameDefine.OnHyperModeEnd);
        }

        public float GetDamage()
        {
            if (m_currentHP > 0)
                return m_damage;
            else return 0;
        }

        public void IdleAnimation()
        {
            m_skeleton.AnimationState.SetAnimation(0, m_idleAnimation, true);
        }

        public void LookLeft()
        {
            m_skeleton.skeleton.ScaleX = -1;
        }

        public void LookRight()
        {
            m_skeleton.skeleton.ScaleX = 1;
        }

        public bool IsPlayerOnTheLeft() => m_playerPos.transform.position.x > transform.position.x;

        public void LookAtPlayer() => m_isLookingAtPlayer = true;

        public void StopLookingAtPlayer() => m_isLookingAtPlayer = false;

        protected virtual void Update()
        {
            if (m_isLookingAtPlayer)
            {
                m_playerPos.transform.position = m_player.position;
                if (IsPlayerOnTheLeft())
                    LookLeft();
                else
                    LookRight();
            }
        }

        public void OnInteractWithTarget()
        {
        }

        public void TakeLethalDamage()
        {
            TakeDamage(m_maxHP * 15 / 100);
        }
    }
}
