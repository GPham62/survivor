using DG.Tweening;
using Sirenix.OdinInspector;
using Spine.Unity;
using System;
using UnityEngine;
using WingsMob.Survival.Combat;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Global;
using WingsMob.Survival.Stats;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Model
{
    public class SurvivorElite : MonoBehaviour, IDamageable, IAttacker
    {
        [Title("Take Damage Effect")]
        [SerializeField] private SpriteRenderer m_sprite;
        [SerializeField] private Color m_takeDamageColor;
        [SerializeField] private float m_takeDamageduration;
        [SerializeField] private AnimationCurve m_animCurve;

        [Title("Gameplay")]
        [SerializeField] protected PlayMakerFSM m_controlFSM;
        [SerializeField] private EnemyAnimationController m_animationController;
        private BaseStats m_stats;
        protected float m_maxHP, m_damage, m_damageRes, m_speed;
        protected float m_currentHP;
        protected Transform m_player;
        protected Rigidbody2D m_rigidBody;
        private Color m_initColor;
        private bool m_isDamageEffectRunning;

        private void OnEnable()
        {
            this.RegisterListener(MethodNameDefine.OnGamePaused, PauseTime);
            this.RegisterListener(MethodNameDefine.OnGameResumed, ResumeTime);
        }

        private void OnDisable()
        {
            this.RemoveListener(MethodNameDefine.OnGamePaused, PauseTime);
            this.RemoveListener(MethodNameDefine.OnGameResumed, ResumeTime);
        }

        protected virtual void PauseTime(object obj)
        {
            m_controlFSM.enabled = false;
        }

        protected virtual void ResumeTime(object obj)
        {
            m_controlFSM.enabled = true;
        }

        public virtual void Init()
        {
            m_controlFSM.enabled = true;
            m_isDamageEffectRunning = false;
            m_stats = GetComponent<BaseStats>();
            m_rigidBody = GetComponent<Rigidbody2D>();
            m_stats.SetLevel(1);
            m_player = LevelManager.Instance.playerController.mover.transform;
            m_maxHP = (m_stats.GetBaseStat(CharacterStats.HealthBase) + m_stats.GetBaseStat(CharacterStats.HealthAddUp) * SurvivorConfig.healthFactor * LevelManager.Instance.scenarioHandler.waveCount) /** (LevelManager.Instance.levelId + 1)*/;
            m_damage = (m_stats.GetBaseStat(CharacterStats.AttackBase) + m_stats.GetBaseStat(CharacterStats.AttackAddUp) * SurvivorConfig.attackFactor * LevelManager.Instance.scenarioHandler.waveCount) /** (LevelManager.Instance.levelId + 1)*/;
            m_speed = m_stats.GetBaseStat(CharacterStats.Speed);
            m_damageRes = m_stats.GetBaseStat(CharacterStats.DamageResist);
            m_animationController.Init();
            m_currentHP = m_maxHP;
            m_initColor = m_sprite.color;
        }

        public float GetDamage()
        {
            if (m_currentHP > 0)
                return m_damage;
            else return 0;
        }

        public void OnInteractWithTarget()
        {

        }

        public bool IsAlive()
        {
            return m_currentHP > 0;
        }

        private float m_damageTemp;

        public virtual void TakeDamage(float damage)
        {
            if (!IsAlive()) return;
            m_damageTemp = m_damageRes > 0 ? damage - m_damageRes * damage : damage;
            m_currentHP = m_currentHP - m_damageTemp < 0 ? 0 : m_currentHP - m_damageTemp;
            GameAssets.Instance.CreateDamagePopup(transform.position, (int)m_damageTemp);
            if (m_currentHP <= 0)
            {
                Die();
            }
            else
            {
                if (m_sprite != null && !m_isDamageEffectRunning)
                {
                    m_isDamageEffectRunning = true;
                    m_sprite.DOColor(m_takeDamageColor, m_takeDamageduration).SetEase(m_animCurve).OnComplete(() => {
                        m_isDamageEffectRunning = false;
                        m_sprite.color = m_initColor;
                    });
                }
            }
        }

        public void KnockBack(Vector2 direction, float strength, float duration)
        {
            m_rigidBody.velocity += direction * strength / 2;
            StartCoroutine(CoroutineUtils.DelayCallback(duration, () => m_rigidBody.velocity = Vector2.zero));
        }

        protected virtual void Update()
        {
            if (IsPlayerOnTheRight())
                LookRight();
            else
                LookLeft(); 
        }

        public bool IsPlayerOnTheRight() => m_player.position.x > transform.position.x;

        public void LookRight() => transform.eulerAngles = new Vector3(0, 0, 0);

        public void LookLeft() => transform.eulerAngles = new Vector3(0, -180, 0);

        protected virtual void Die()
        {
            m_controlFSM.enabled = false;
            LevelManager.Instance.collectableSpawner.SpawnWeaponChest(transform.position, UnityEngine.Random.Range(0f, 100f));
            m_animationController.Death(transform, null, false);
        }

        public virtual Vector2 GetNextMovePos() => Vector2.MoveTowards(transform.position, m_player.position, m_speed * Time.deltaTime);

        public Vector3 GetPlayerPos() => m_player.position;

        public void TakeLethalDamage()
        {
            m_damageTemp = m_maxHP / 3;
            m_currentHP = m_currentHP - m_damageTemp < 0 ? 0 : m_currentHP - m_damageTemp;
            GameAssets.Instance.CreateDamagePopup(transform.position, (int)m_damageTemp);
            if (m_currentHP <= 0)
            {
                Die();
            }
        }
    }
}
