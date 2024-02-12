using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Global;
using WingsMob.Survival.Stats;

namespace WingsMob.Survival.Model
{
    [RequireComponent(typeof(WeaponStats))]
    public abstract class Weapon : SkillCard
    {
        [SerializeField] protected WeaponStats m_weaponStats;
        [ReadOnly] public bool isFinalForm;
        protected string m_tweenAttackId;
        protected LevelManager m_levelManager;

        public virtual void Init(Action<string> callback)
        {
            m_levelManager = LevelManager.Instance;
            m_weaponStats.Init();
            m_weaponStats.level = 1;
            isFinalForm = false;
            CanLevelUp = true;
            callback.Invoke(m_tweenAttackId);
        }

        public virtual void ResetStats() { }

        public override int GetId() => GetWeaponId();

        public override int GetCardLevel() => GetWeaponLevel();

        protected virtual void OnDestroy()
        {
            DOTween.Kill(m_tweenAttackId);
        }

        public virtual void LevelUp()
        {
            if (m_weaponStats.level + 1 > SurvivorConfig.MaxWeaponLevel)
            {
                UltimateUpgrade();
            }
            else
            {
                NormalLevelUp();
            }
            m_levelManager.skillCollection.UpdateSkillChance(this);
        }

        public virtual void NormalLevelUp()
        {
            m_weaponStats.level++;
            if (m_weaponStats.level >= SurvivorConfig.MaxWeaponLevel)
            {
                if (!LevelManager.Instance.playerController.weaponAugmentorManager.IsWeaponUpgradable(GetWeaponId()))
                    CanLevelUp = false;
            }
        }

        public virtual void UltimateUpgrade()
        {
            isFinalForm = true;
            CanLevelUp = false;
            m_weaponStats.UpgradeStatsToUltimate();
        }

        public int GetWeaponId() => m_weaponStats.GetWeaponId();

        public int GetWeaponLevel() => m_weaponStats.level;

        public string GetWeaponPreviewName() => m_weaponStats.GetWeaponPreviewName();

        public string GetWeaponNameByLevel(int level) => m_weaponStats.GetWeaponNameByLevel(level);

        public string GetWeaponPreviewDescription() => m_weaponStats.GetWeaponPreviewDescription();

        public string GetWeaponDescriptionByLevel(int level) => m_weaponStats.GetWeaponDescriptionByLevel(level);

        public Sprite GetWeaponPreviewIcon() => m_weaponStats.GetWeaponPreviewIcon();

        public Sprite GetWeaponIconByLevel(int level) => m_weaponStats.GetWeaponIconByLevel(level);

        public Sprite GetWeaponIcon() => m_weaponStats.GetWeaponIcon();

        public bool IsWeaponPreviewUltimate() => m_weaponStats.IsWeaponReachMaxLevel();
    }
}
