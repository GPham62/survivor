using DarkTonic.PoolBoss;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Global;
using WingsMob.Survival.Stats;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Combat
{
    public abstract class Fighter : MonoBehaviour, IDamageable
    {
        [Title("Take Damage Effect")]
        [SerializeField] private SpriteRenderer m_sprite;
        [SerializeField] private Color m_takeDamageColor;
        [SerializeField] private float m_takeDamageduration;
        [SerializeField] private AnimationCurve m_animCurve;
        private bool m_isDamageEffectRunning;
        private Color m_initColor;
        protected float m_health;
        protected float m_maxHealth;
        protected float m_damage;
        protected float m_damageRes;
        protected float m_critDamage;
        protected float m_critRate;
        public bool isDead;
        protected IAnimationController m_animationController;
        
        public virtual void Init(IAnimationController animationController, BaseStats stats)
        {
            m_animationController = animationController;
            if (m_sprite != null)
                m_initColor = m_sprite.color;
            Reset(stats);
        }

        public virtual void UpdateStats(BaseStats stats)
        {
            int waveCount = LevelManager.Instance.scenarioHandler.waveCount;
            int level = LevelManager.Instance.levelId + 1;
            m_maxHealth = (stats.GetBaseStat(CharacterStats.HealthBase) + stats.GetBaseStat(CharacterStats.HealthAddUp) * waveCount * SurvivorConfig.healthFactor) * Mathf.Pow(1.35f, level);
            m_damage = (stats.GetBaseStat(CharacterStats.AttackBase) + stats.GetBaseStat(CharacterStats.AttackAddUp) * waveCount * SurvivorConfig.attackFactor) * Mathf.Pow(1.42f,level);
            m_damageRes = stats.GetBaseStat(CharacterStats.DamageResist);
            m_critDamage = stats.GetBaseStat(CharacterStats.CritDamage);
            m_critRate = stats.GetBaseStat(CharacterStats.CritRate);
        }

        public virtual void Reset(BaseStats stats)
        {
            UpdateStats(stats);
            m_health = m_maxHealth;
            isDead = false;
            m_isDamageEffectRunning = false;
            if (m_sprite != null)
                m_sprite.color = m_initColor;
        }

        private float m_damageTemp;

        public virtual void TakeDamage(float damage)
        {
            if (GameStatus.CurrentState != GameState.Playing || m_health < 0 || isDead)
                return;

            m_damageTemp = m_damageRes > 0 ? damage - m_damageRes * damage : damage;
            m_health = m_health - m_damageTemp < 0 ? 0 : m_health - m_damageTemp;
            DisplayDamage((int)m_damageTemp);

            if (m_health <= 0)
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

        public void TakeLethalDamage()
        {
            DisplayDamage((int)m_health);
            m_health = 0;
            Die();
        }

        protected virtual void DisplayDamage(int damage){ }

        public virtual void Die() 
        { 
            isDead = true;
        }

        public virtual void KnockBack(Vector2 direction, float strength, float duration)
        {
        }
    }
}
