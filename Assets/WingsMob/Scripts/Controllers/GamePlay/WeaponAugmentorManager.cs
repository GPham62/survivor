using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WingsMob.Survival.Global;
using WingsMob.Survival.Model;

namespace WingsMob.Survival.Controller
{
    public class WeaponAugmentorManager : MonoBehaviour
    {
        [ReadOnly] public List<WeaponAugmentor> weaponAugmentors;
        
        public List<int> upgradableWeaponsId;

        public void Init()
        {
            weaponAugmentors = new List<WeaponAugmentor>();
            upgradableWeaponsId = new List<int>();
        }

        public void SpawnAugmentor(WeaponAugmentor augmentor)
        {
            if (IsAllAugmentorFilled())
                return;
            WeaponAugmentor newAugmentor = Instantiate(augmentor, transform, false);
            newAugmentor.Init();
            weaponAugmentors.Add(newAugmentor);
            upgradableWeaponsId.Add(newAugmentor.GetUpgradableWeaponId());
            Weapon weapon = LevelManager.Instance.playerController.weaponManager.weaponsArr.SingleOrDefault(w => w != null && w.GetWeaponId() == newAugmentor.GetUpgradableWeaponId());
            if (weapon != null)
                weapon.CanLevelUp = true;
            LevelManager.Instance.skillCollection.UpdateSkillChance(newAugmentor);
        }

        public bool IsAllAugmentorFilled() => weaponAugmentors.Count >= SurvivorConfig.NumOfAugmentor;

        public bool IsWeaponUpgradable(int weaponId) => weaponAugmentors.Any(augmentor => augmentor.GetUpgradableWeaponId() == weaponId);
        
        public bool CanAugmentorLevelUp()
        {
            if (!IsAllAugmentorFilled())
                return true;

            return IsObtainedAugmentorsUpgradable();
        }

        public bool IsObtainedAugmentorsUpgradable()
        {
            for (int i = 0; i < weaponAugmentors.Count; i++)
            {
                if (weaponAugmentors[i].CanLevelUp)
                    return true;
            }
            return false;
        }
    }
}