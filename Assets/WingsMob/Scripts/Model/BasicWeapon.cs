using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Combat;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Global;

namespace WingsMob.Survival.Model
{
    public class BasicWeapon : Weapon
    {
        private WeaponReloader m_weaponReloader;

        public override void Init(Action<string> callback)
        {
            m_weaponReloader = LevelManager.Instance.playerController.weaponManager.weaponReloader;
            m_tweenAttackId = m_weaponReloader.name;
            base.Init(callback);
            
            isFinalForm = false;
            m_weaponReloader.SetReloadTime(m_weaponStats.GetWeaponParam(WeaponParam.RechargeTime));
            m_weaponReloader.Reset();
            this.RegisterListener(MethodNameDefine.OnWeaponReloaded, OnWeaponReloaded);
        }

        public override void ResetStats()
        {
            m_weaponReloader.Reset();
        }

        private void OnWeaponReloaded(object senderParam = null)
        {
            if (isFinalForm || GameStatus.CurrentState != GameState.Playing)
                return;

            NormalAttack();
        }

        public override void NormalLevelUp()
        {
            base.NormalLevelUp();
            m_weaponReloader.SetReloadTime(m_weaponStats.GetWeaponParam(WeaponParam.RechargeTime));
            m_weaponReloader.Reset();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (!isFinalForm)
                this.RemoveListener(MethodNameDefine.OnWeaponReloaded, OnWeaponReloaded);
        }

        public override void UltimateUpgrade()
        {
            base.UltimateUpgrade();

            isFinalForm = true;

            m_rechargeTimeUltimate = m_weaponStats.GetWeaponParam(WeaponParam.RechargeTime);

            this.RemoveListener(MethodNameDefine.OnWeaponReloaded, OnWeaponReloaded);

            m_weaponReloader.Hide();

            WeaponManager weaponManager = m_levelManager.playerController.weaponManager;

            weaponManager.UnsubscribeFromTweenList(m_weaponReloader.name);

            weaponManager.SubscribeToTweenList(name);

            m_tweenAttackId = name;

            Sequence ultiSequence = DOTween.Sequence();

            ultiSequence.SetId(m_tweenAttackId).SetLoops(-1, LoopType.Restart).AppendCallback(UltiAttack).AppendInterval(m_rechargeTimeUltimate);
        }

        private float m_rechargeTimeUltimate;

        protected virtual void NormalAttack() { }

        protected virtual void UltiAttack() { }
    }
}
