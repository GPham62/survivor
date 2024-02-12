using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Controller;
using WingsMob.Survival.GameConfig;
using WingsMob.Survival.Global;

namespace WingsMob.Survival.Model
{
    public class WeaponAugmentor : SkillCard
    {
        [SerializeField] WeaponAugmentorConfig config;

        [SerializeField] private int m_level = 0;

        public void Init()
        {
            m_level = 1;
            CanLevelUp = true;
            IncreasePlayerStats();
        }

        public void LevelUp() { 
            if (m_level < SurvivorConfig.MaxAugmentorLevel)
            {
                m_level++;
                if (m_level >= SurvivorConfig.MaxAugmentorLevel)
                    CanLevelUp = false;
                IncreasePlayerStats();
                LevelManager.Instance.skillCollection.UpdateSkillChance(this);
            }  
        }

        private void IncreasePlayerStats()
        {
            PlayerController playerController = LevelManager.Instance.playerController;
            switch (GetStats())
            {
                case CharacterStats.AttackBase:
                    playerController.fighter.Damage = playerController.fighter.baseDamage + playerController.fighter.baseDamage * GetStatsValue() / 100;
                    break;
                case CharacterStats.Speed:
                    playerController.mover.Speed = playerController.mover.baseSpeed + playerController.mover.baseSpeed * GetStatsValue() / 100;
                    break;
                case CharacterStats.Regen:
                    playerController.fighter.regen = GetStatsValue() / 100;
                    break;
                case CharacterStats.WeaponDuration:
                    playerController.fighter.weaponDuration = GetStatsValue() / 100;
                    break;
                case CharacterStats.MoneyAmp:
                    LevelManager.Instance.gameStats.moneyAmp = GetStatsValue() / 100;
                    break;
                case CharacterStats.ExperienceAmp:
                    LevelManager.Instance.gameStats.expAmp = GetStatsValue() / 100;
                    break;
                case CharacterStats.HealthBase:
                    playerController.fighter.MaxHealth = playerController.fighter.baseMaxHealth + playerController.fighter.baseMaxHealth * GetStatsValue() / 100;
                    break;
                case CharacterStats.CooldownReduction:
                    playerController.fighter.cooldownReduction = GetStatsValue() / 100;
                    break;
                case CharacterStats.CollectRange:
                    playerController.itemCollector.IncreaseSize(GetStatsValue());
                    break;
                case CharacterStats.WeaponAoe:
                    playerController.fighter.weaponAoe = GetStatsValue() / 100;
                    break;
            }
            this.PostEvent(MethodNameDefine.OnStatsChanged);
        }

        public override int GetCardLevel() => GetAugmentorLevel();

        public override int GetId() => GetAugmentorId();

        public string GetAugmentorName() => config.augmentorName;

        public int GetAugmentorId() => config.augmentorId;

        public Sprite GetAugmentorIcon() => config.augmentorIcon;

        public string GetAugmentorDescription() => config.augmentorDescription;

        public int GetAugmentorLevel() => m_level;

        public CharacterStats GetStats() => config.stats;

        public float GetStatsValue() => config.baseValue + (m_level - 1) * config.upgradeValue;

        public int GetUpgradableWeaponId() => config.weaponId;

        public bool IsForBasicWeapon() => config.isBasicWeapon;
    }
}
