using UnityEngine;
using WingsMob.Survival.GameConfig;
using WingsMob.Survival.Global;
using WingsMob.Survival.Model;

namespace WingsMob.Survival.Stats
{
    public class WeaponStats : MonoBehaviour
    {
        public int level;
        [SerializeField] private NormalWeaponConfig m_normalWeaponConfig;
        [SerializeField] private UltiWeaponConfig m_ultiWeaponConfig;

        private WeaponConfig m_currentWeaponConfig;

        public void Init()
        {
            if (m_currentWeaponConfig == null)
            {
                level = 0;
                m_currentWeaponConfig = m_normalWeaponConfig;
            }
        }

        public void UpgradeStatsToUltimate()
        {
            level = 1;
            m_currentWeaponConfig = m_ultiWeaponConfig;
        }

        public float GetWeaponParam(WeaponParam weaponParam)
        {
            return m_currentWeaponConfig.GetParam(weaponParam, level);
        }

        public bool IsWeaponReachMaxLevel() => level == SurvivorConfig.MaxWeaponLevel;

        public string GetWeaponPreviewDescription()
        {
            Init();
            return IsWeaponReachMaxLevel() ? m_ultiWeaponConfig.GetDescription(level) : m_currentWeaponConfig.GetDescription(level + 1);
        }

        public string GetWeaponDescriptionByLevel(int level)
        {
            return level >= SurvivorConfig.MaxWeaponLevel ? m_ultiWeaponConfig.GetDescription(0) : m_currentWeaponConfig.GetDescription(level);
        }

        public string GetWeaponPreviewName()
        {
            Init();
            return IsWeaponReachMaxLevel() ? m_ultiWeaponConfig.weaponName : m_currentWeaponConfig.weaponName;
        }

        public string GetWeaponNameByLevel(int level)
        {
            return level >= SurvivorConfig.MaxWeaponLevel ? m_ultiWeaponConfig.weaponName : m_currentWeaponConfig.weaponName;
        }

        public Sprite GetWeaponIcon() => m_currentWeaponConfig.weaponIcon;

        public Sprite GetWeaponPreviewIcon()
        {
            Init();
            return IsWeaponReachMaxLevel() ? m_ultiWeaponConfig.weaponIcon : GetWeaponIcon();
        }

        public Sprite GetWeaponIconByLevel(int level)
        {
            return level >= SurvivorConfig.MaxWeaponLevel ? m_ultiWeaponConfig.weaponIcon : m_currentWeaponConfig.weaponIcon;
        }

        public int GetWeaponId()
        {
            Init();
            return m_currentWeaponConfig.weaponId;
        }

        public WeaponProjectile GetWeaponProjectile()
            => m_currentWeaponConfig.hasProjectile && m_currentWeaponConfig.projectilePrefab != null ?
                                                         m_currentWeaponConfig.projectilePrefab : null;

        public WeaponProjectileParticle GetWeaponProjectileParticle()
            => m_currentWeaponConfig.hasProjectile && m_currentWeaponConfig.projectileParticlePrefab != null ?
                                                         m_currentWeaponConfig.projectileParticlePrefab : null;
    }
}
