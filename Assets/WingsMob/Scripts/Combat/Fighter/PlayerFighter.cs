using DarkTonic.PoolBoss;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Global;
using WingsMob.Survival.Stats;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Combat
{
    public class PlayerFighter : Fighter
    {
        public float baseDamage { get; private set; }
        public float Damage
        {
            get { return m_damage; }
            set { m_damage = value; }
        }

        public float baseMaxHealth { get; private set; }

        public float MaxHealth
        {
            get { return m_maxHealth; }
            set { m_maxHealth = value; }
        }

        [HideInInspector] public float weaponAoe;
        [HideInInspector] public float weaponDuration;
        [HideInInspector] public float regen;
        [HideInInspector] public float cooldownReduction;

        [SerializeField] private PlayerHealthUI m_healthUI;
        [SerializeField] private int m_regenDuration = 5;

        private float m_regenCountdown;
        private PlayerController m_playerController;
        
        public void InitPlayer(PlayerController playerController)
        {
            Init(playerController.animController, playerController.playerBaseStats);
            baseDamage = m_damage;
            baseMaxHealth = m_maxHealth;
            m_playerController = playerController;
            weaponDuration = 0f;
            regen = 0f;
        }

        public override void UpdateStats(BaseStats stats)
        {
            m_damage = stats.GetBaseStat(CharacterStats.AttackBase);
            m_maxHealth = stats.GetBaseStat(CharacterStats.HealthBase);
            m_damageRes = stats.GetBaseStat(CharacterStats.DamageResist);
        }

        public override void Reset(BaseStats stats)
        {
            base.Reset(stats);
            m_healthUI.UpdateHealthUI(m_health / m_maxHealth);
        }

        private void Update()
        {
            if (GameStatus.CurrentState != GameState.Playing)
                return;
            m_regenCountdown += Time.deltaTime;
            if (regen > 0 && m_regenCountdown > m_regenDuration)
            {
                m_regenCountdown = 0;
                GainHealth(regen * m_maxHealth);
            }
        }

        public void GainHealth(float healthAmount)
        {
            m_health = m_health + healthAmount > m_maxHealth ? m_maxHealth : m_health + healthAmount;
            m_healthUI.UpdateHealthUI(m_health / m_maxHealth);
        }

        protected override void DisplayDamage(int damage)
        {
            //SoundManager.Instance.PlaySound(MusicNameDefine.GetCombatPlayerDamaged());
            PoolBoss.SpawnInPool(GameAssets.Instance.playerTakeDamageEffect, transform.position, transform.rotation);
            m_healthUI.UpdateHealthUI(m_health / m_maxHealth);
        }

        public override void Die()
        {
            base.Die();

            m_playerController.HandleInput(State.PlayerInput.Death);
            m_playerController.Act();
        }
    }
}
