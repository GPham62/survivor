using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Combat;
using WingsMob.Survival.Global;
using WingsMob.Survival.Model;

namespace WingsMob.Survival.Controller
{
    public class WeaponManager : MonoBehaviour
    {
        public WeaponReloader weaponReloader;
        [ReadOnly] public Weapon[] weaponsArr;
        [ReadOnly] public BasicWeapon basicWeapon;
        [SerializeField] [ReadOnly] private List<string> weaponsTweenList;

        public int weaponNextIndex { get; private set; }
        public List<int> deletedWeaponIds;

        public void Init()
        {
            weaponsTweenList = new List<string>();
            deletedWeaponIds = new List<int>();
            //ProfileManager.Instance.UserData.BasicWeaponId = 0;
            weaponsArr = new Weapon[SurvivorConfig.NumOfPlayerWeapons];
            weaponNextIndex = 0;
            this.RegisterListener(MethodNameDefine.OnGamePaused, PauseAllWeaponsTween);
            this.RegisterListener(MethodNameDefine.OnGameResumed, RestartAllWeaponsTween);
            this.RegisterListener(MethodNameDefine.OnGameOver, PauseAllWeaponsTween);
            this.RegisterListener(MethodNameDefine.OnStatsChanged, ResetAllWeaponsStats);
            SpawnBasicWeapon(GameAssets.Instance.GetBasicWeaponById(0));
        }

        private void OnDisable()
        {
            this.RemoveListener(MethodNameDefine.OnGamePaused, PauseAllWeaponsTween);
            this.RemoveListener(MethodNameDefine.OnGameResumed, RestartAllWeaponsTween);
            this.RemoveListener(MethodNameDefine.OnGameOver, PauseAllWeaponsTween);
            this.RemoveListener(MethodNameDefine.OnStatsChanged, ResetAllWeaponsStats);
        }

        private void PauseAllWeaponsTween(object obj = null)
        {
            foreach (var weaponTweenId in weaponsTweenList)
            {
                DOTween.Pause(weaponTweenId);
            }
        }

        private void RestartAllWeaponsTween(object obj = null)
        {
            List<Tween> pausedTweens = DOTween.PausedTweens();
            if (pausedTweens == null) return;

            foreach (var pausedTween in pausedTweens)
            {
                if (weaponsTweenList.Contains(pausedTween.stringId))
                {
                    pausedTween.TogglePause();
                }
            }
        }


        private void ResetAllWeaponsStats(object obj)
        {;
            foreach (var weapon in weaponsArr)
            {
                if (weapon != null)
                    weapon.ResetStats();
            }
        }

        public void SpawnBasicWeapon(BasicWeapon weapon)
        {
            basicWeapon = Instantiate(weapon, transform, false);
            basicWeapon.Init(SubscribeToTweenList);
            weaponsArr[weaponNextIndex++] = basicWeapon;
            LevelManager.Instance.skillCollection.UpdateSkillChance(basicWeapon);
        }

        public void SpawnNewWeapon(Weapon weapon)
        {
            if (IsAllWeaponFilled())
            {
                Common.LogError("Something wrong here!");
                return;
            }

            Weapon newWeapon = Instantiate(weapon, transform, false);
            if (deletedWeaponIds.Count < 1)
                weaponsArr[weaponNextIndex++] = newWeapon;
            else
            {
                int newId = deletedWeaponIds.Pop();
                weaponsArr[newId] = newWeapon;
            }
            newWeapon.Init(SubscribeToTweenList);
            LevelManager.Instance.skillCollection.UpdateSkillChance(newWeapon);
        }

        public void DeleteWeapon(Weapon weapon)
        {
            deletedWeaponIds.Add(Array.IndexOf(weaponsArr, weapon));
            weaponsArr[Array.IndexOf(weaponsArr, weapon)] = null;
            Destroy(weapon.gameObject);
            LevelManager.Instance.skillCollection.UpdateWeaponCollection();
        }

        public void SubscribeToTweenList(string tweenId)
        {
            if (tweenId != null)
                weaponsTweenList.Add(tweenId);
        }

        public void UnsubscribeFromTweenList(string tweenId) => weaponsTweenList.Remove(tweenId);

        public bool CanWeaponLevelUp()
        {
            if (!IsAllWeaponFilled())
                return true;

            return IsObtainedWeaponsUpgradable();
        }

        public bool IsObtainedWeaponsUpgradable()
        {
            for (int i = 0; i < weaponsArr.Length; i++)
            {
                if (weaponsArr[i] != null && weaponsArr[i].CanLevelUp)
                    return true;
            }
            return false;
        }

        public bool IsAllWeaponFilled()
        {
            if (deletedWeaponIds.Count < 1)
                return weaponNextIndex >= weaponsArr.Length;
            else
                return false;
        }
    }
}
