using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Global;

namespace WingsMob.Survival.Combat
{
    public class WeaponReloader : MonoBehaviour
    {
        [SerializeField] private float m_reloadTime = 0.1f;

        private Image m_weaponReloadImg;

        private void Awake()
        {
            m_weaponReloadImg = GetComponent<Image>();
        }

        private void SetNewReloadTween()
        {
            m_weaponReloadImg.fillAmount = 0f;
            m_weaponReloadImg.DOFillAmount(1, m_reloadTime * (1 - LevelManager.Instance.playerController.fighter.cooldownReduction)).SetId(name).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart)
                .OnStepComplete(() =>
                {
                    if (gameObject.activeSelf)
                    {
                        this.PostEvent(MethodNameDefine.OnWeaponReloaded);
                    }
                });
        }

        private void OnDestroy()
        {
            KillPreviousTween();
        }

        public void SetReloadTime(float reloadTime)
        {
            m_reloadTime = reloadTime;
        }

        public void Hide()
        {
            KillPreviousTween();
            gameObject.SetActive(false);
        }

        public void Reset()
        {
            KillPreviousTween();
            SetNewReloadTween();
        }

        private void KillPreviousTween()
        {
            DOTween.Kill(name, false);
        }
    }
}
