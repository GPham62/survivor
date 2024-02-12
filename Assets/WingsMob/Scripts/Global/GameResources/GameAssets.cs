using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Model;
using Sirenix.OdinInspector;
using System.Linq;
using DarkTonic.PoolBoss;
using WingsMob.Survival.Combat;
using DamageNumbersPro;

namespace WingsMob.Survival.Global
{
    public class GameAssets : MonoBehaviour
    {
        private static GameAssets m_instance;

        public static GameAssets Instance
        {
            get
            {
                if (m_instance == null) m_instance = Instantiate(Resources.Load<GameAssets>("GameAssets"));
                return m_instance;
            }
        }

        #region public
        [Title("Popup")]
        [PreviewField] [AssetsOnly] public DamageNumber damagePopupPrefab;
        [PreviewField] [AssetsOnly] public DamageNumber healPopupPrefab;
        [PreviewField] [AssetsOnly] public DamageNumber coinsPopupPrefab;
        [PreviewField] [AssetsOnly] public GameObject levelUpPopupPrefab;
        [PreviewField] [AssetsOnly] public GameObject levelUpAdsPopupPrefab;
        [PreviewField] [AssetsOnly] public GameObject winPopupPrefab;
        [PreviewField] [AssetsOnly] public GameObject losePopupPrefab;
        [PreviewField] [AssetsOnly] public GameObject pausePopupPrefab;
        [PreviewField] [AssetsOnly] public GameObject adsRewardPopupPrefab;
        [PreviewField] [AssetsOnly] public GameObject slotMachinePopupPrefab;
        [PreviewField] [AssetsOnly] public GameObject gameSettingsPrefab;

        //[Title("Environment")]
        //[PreviewField] [AssetsOnly] public Transform tileMap;

        [Title("Collectables")]
        [PreviewField] [AssetsOnly] public Transform diamondBlue;
        [PreviewField] [AssetsOnly] public Transform diamondYellow;
        [PreviewField] [AssetsOnly] public Transform diamondGreen;
        [PreviewField] [AssetsOnly] public Transform meat;
        [PreviewField] [AssetsOnly] public Transform coin;
        [PreviewField] [AssetsOnly] public Transform coinPack;
        [PreviewField] [AssetsOnly] public Transform magnet;
        [PreviewField] [AssetsOnly] public Transform bomb;
        [PreviewField] [AssetsOnly] public Transform weaponChest;
        [PreviewField] [AssetsOnly] public Transform weaponAdsChest;
        [PreviewField] [AssetsOnly] public Transform supplyBox;

        [Title("Effects")]
        [PreviewField] [AssetsOnly] public Transform enemyDeadEffect;
        [PreviewField] [AssetsOnly] public Transform playerTakeDamageEffect;
        [PreviewField] [AssetsOnly] public Transform collectableAfterEffect;

        [Title("Card")]
        [PreviewField] [AssetsOnly] public Sprite cardNormal;
        [PreviewField] [AssetsOnly] public Sprite cardUltimate;
        [PreviewField] [AssetsOnly] public Sprite cardAugmentor;
        [PreviewField] [AssetsOnly] public Sprite starEmpty;
        [PreviewField] [AssetsOnly] public Sprite starFilled;
        [PreviewField] [AssetsOnly] public Sprite starUltimate;

        [Title("Card", "Reward")]
        [PreviewField] [AssetsOnly] public Sprite cardRewardNormal;
        [PreviewField] [AssetsOnly] public Sprite cardRewardUltimate;
        [PreviewField] [AssetsOnly] public Sprite cardRewardAugmentor;

        [Title("Weapon Collection")]
        [AssetsOnly] public BasicWeapon[] basicWeaponCollection;
        [AssetsOnly] public Weapon[] weaponCollection;

        [Title("Weapon Augmentor Collection")]
        [AssetsOnly] public WeaponAugmentor[] weaponAugmentorCollection;

        [Title("Monster Library")]
        [AssetsOnly] public MonsterLibrary monsterLibrary;

        [Title("Boss Library")]
        [AssetsOnly] public BossLibrary bossLibrary;

        [Title("Elite Library")]
        [AssetsOnly] public EliteLibrary eliteLibrary;
        #endregion
        public BasicWeapon GetBasicWeaponById(int basicWeaponId) => basicWeaponCollection.FirstOrDefault(weapon => weapon.GetWeaponId() == basicWeaponId);

        public Weapon GetWeaponById(int weaponId) => weaponCollection.FirstOrDefault(weapon => weapon.GetWeaponId() == weaponId);

        private List<int> m_weaponIds = new List<int>();

        public List<Weapon> GetUpgradableWeapons(Weapon[] playerWeapons, List<WeaponAugmentor> playerAugmentors)
        {
            //Get upgradable weapons in player equipments
            List<Weapon> upgradableWeapons = new List<Weapon>();
            m_weaponIds.Clear();
            for (int i = 0; i < playerWeapons.Length; i++)
            {
                if (playerWeapons[i] == null)
                    continue;
                m_weaponIds.Add(playerWeapons[i].GetWeaponId());
                if (playerWeapons[i].CanLevelUp)
                {
                    upgradableWeapons.Add(playerWeapons[i]);
                }
            }
            //Get remaining weapons in collection 
            if (playerWeapons.Any(w => w != null && w.GetWeaponId() == 7 && w.isFinalForm))
                m_weaponIds.Add(8);

            List<Weapon> remainingWeaponsInCollection = weaponCollection.Where(weapon => !m_weaponIds.Contains(weapon.GetWeaponId())).ToList();
            upgradableWeapons.AddRange(remainingWeaponsInCollection);
            return upgradableWeapons;
        }

        private List<int> m_augmentorIds = new List<int>();
        public List<WeaponAugmentor> GetUpgradableAugmentors(List<WeaponAugmentor> playerAugmentors)
        {
            List<WeaponAugmentor> upgradableAugmentors = new List<WeaponAugmentor>();
            m_augmentorIds.Clear();
            for (int i = 0; i < playerAugmentors.Count; i++)
            {
                m_augmentorIds.Add(playerAugmentors[i].GetAugmentorId());
                if (playerAugmentors[i].CanLevelUp)
                    upgradableAugmentors.Add(playerAugmentors[i]);
            }
            List<WeaponAugmentor> remainingAugmentorsInCollection = weaponAugmentorCollection.Where(augmentor => !m_augmentorIds.Contains(augmentor.GetAugmentorId())).ToList();
            upgradableAugmentors.AddRange(remainingAugmentorsInCollection);
            return upgradableAugmentors;
        }
        
        public WeaponAugmentor GetAugmentorById(int id) => weaponAugmentorCollection.FirstOrDefault(augmentor => augmentor.GetAugmentorId() == id);

        public void CreateDamagePopup(Vector3 position, int damageAmount, bool isFollow = false, Transform followTarget = null)
        {
            if (Application.targetFrameRate > 30f)
            {
                DamageNumber newDamageNumber = damagePopupPrefab.Spawn(position, damageAmount);
                if (isFollow)
                {
                    newDamageNumber.SetFollowedTarget(followTarget);
                }
            }
        }

        public void CreateHealPopup(Vector3 position, int healAmount, bool isFollow = false, Transform followTarget = null)
        {
            DamageNumber newDamageNumber = healPopupPrefab.Spawn(position, healAmount);
            if (isFollow)
            {
                newDamageNumber.SetFollowedTarget(followTarget);
            }
        }

        public void CreateCoinsPopup(Vector3 position, int coinsAmount, bool isFollow = false, Transform followTarget = null)
        {
            DamageNumber newDamageNumber = coinsPopupPrefab.Spawn(position, coinsAmount);
            if (isFollow)
            {
                newDamageNumber.SetFollowedTarget(followTarget);
            }
        }

        public Transform GetMonsterById(int monsterId) => monsterLibrary[monsterId];

        public SurvivorBoss GetBossById(int bossId) => bossLibrary[bossId];

        public SurvivorElite GetEliteById(int eliteId) => eliteLibrary[eliteId];
    }

    [Serializable]
    public class MonsterLibrary : SerializableDictionaryBase<int, Transform> { }
    [Serializable]
    public class BossLibrary : SerializableDictionaryBase<int, SurvivorBoss> { }

    [Serializable]
    public class EliteLibrary : SerializableDictionaryBase<int, SurvivorElite> { }
}