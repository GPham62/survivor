using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Global;
using WingsMob.Survival.Utils;
using WingsMob.Survival.Model;
using WingsMob.Survival.Controller;

namespace WingsMob.Survival.Combat
{
    public class FidgetSpinner : Weapon
    {
        [SerializeField] private float m_expandTime = 1f;
        [SerializeField] private float m_shrinkTime = 1f;

        private List<FidgetSpinnerProjectile> m_projectileList;
        private Vector2 m_projectileOriginalScale;

        public override void Init(Action<string> callback)
        {
            m_tweenAttackId = name;
            base.Init(callback);
            m_projectileList = new List<FidgetSpinnerProjectile>();
            m_projectileOriginalScale = m_weaponStats.GetWeaponProjectile().transform.localScale;
            UpdateVariables(true);
            SpawnFidgetSpinners();
            SetFidgetSpinnersDamage();
            StartSpinSequence();
        }

        public override void ResetStats()
        {
            if (!isFinalForm)
            {
                DOTween.Kill(name, false);
                StartSpinSequence();
            }
        }

        private float m_projectileDamage, m_duration, m_rechargeTime, m_projectileSpeed, m_projectileAmount, m_orbitRadius;
        private List<Vector3> m_fidgetEndPositions;

        private void UpdateVariables(bool isNormalUpdate)
        {
            if (isNormalUpdate)
            {
                m_duration = m_weaponStats.GetWeaponParam(WeaponParam.Duration);
                m_rechargeTime = m_weaponStats.GetWeaponParam(WeaponParam.RechargeTime);
            }
            m_projectileDamage = m_weaponStats.GetWeaponParam(WeaponParam.Damage) / 100;
            m_projectileSpeed = m_weaponStats.GetWeaponParam(WeaponParam.ProjectileSpeed);
            m_projectileAmount = m_weaponStats.GetWeaponParam(WeaponParam.ProjectileAmount);
            m_orbitRadius = m_weaponStats.GetWeaponParam(WeaponParam.OrbitRadius);
            m_fidgetEndPositions = GenerateFidgetPositionsAroundPlayer(m_projectileAmount);
        }

        private List<Vector3> GenerateFidgetPositionsAroundPlayer(float amount)
        {
            float angle = 0; // start angle
            float angleDelta = 360f / amount;
            List<Vector3> positions = new List<Vector3>();

            while (amount > 0)
            {
                positions.Add(Utilities.CircleXY(m_orbitRadius, angle));
                angle += angleDelta;
                amount--;
            }
            return positions;
        }

        private void SpawnFidgetSpinners()
        {
            for (int i = m_projectileList.Count; i < m_projectileAmount; i++)
            {
                FidgetSpinnerProjectile newProjectile = (FidgetSpinnerProjectile)Instantiate(m_weaponStats.GetWeaponProjectile(), transform);
                newProjectile.transform.localScale = Vector2.zero;
                newProjectile.gameObject.SetActive(false);
                m_projectileList.Add(newProjectile);
            }
        }

        private void SetFidgetSpinnersDamage()
        {
            foreach (var projectile in m_projectileList)
            {
                projectile.SetDamage(m_levelManager.playerController.fighter.Damage * m_projectileDamage);
            }
        }

        private void StartSpinSequence()
        {
            Sequence spinnerSequence = DOTween.Sequence();
            float durationWithReduction = m_duration * (1 + LevelManager.Instance.playerController.fighter.weaponDuration);
            spinnerSequence.AppendCallback(() => OpenSpinner())
                           .AppendInterval(m_expandTime)
                           .Append(transform.DOLocalRotate(new Vector3(0, 0, m_projectileSpeed * durationWithReduction), durationWithReduction, RotateMode.FastBeyond360).SetEase(Ease.Linear))
                           .AppendCallback(() => CloseSpinner())
                           .AppendInterval(m_shrinkTime)
                           .AppendInterval(m_rechargeTime * (1 - LevelManager.Instance.playerController.fighter.cooldownReduction))
                           .SetId(name)
                           .SetLoops(-1, LoopType.Restart);
        }

        private void OpenSpinner(Action onOpenedCallback = null)
        {
            bool callbackUsed = true;
            if (onOpenedCallback != null)
            {
                callbackUsed = false;
            }
            for (int i = 0; i < m_projectileAmount; i++)
            {
                Transform projectileTransform = m_projectileList[i].transform;
                projectileTransform.gameObject.SetActive(true);
                projectileTransform.DOScale(m_projectileOriginalScale, m_expandTime).SetEase(Ease.Linear);
                projectileTransform.DOLocalMove(m_fidgetEndPositions[i], m_expandTime).SetEase(Ease.Linear).OnComplete(() =>
                {
                    if (!callbackUsed)
                    {
                        callbackUsed = true;
                        onOpenedCallback.Invoke();
                    }
                });
            }
        }

        private void CloseSpinner(Action onClosedCallback = null)
        {
            bool callbackUsed = true;
            if (onClosedCallback != null)
            {
                callbackUsed = false;
            }

            for (int i = 0; i < m_projectileAmount; i++)
            {
                Transform projectileTransform = m_projectileList[i].transform;
                projectileTransform.DOScale(Vector2.zero, m_shrinkTime).SetEase(Ease.Linear);
                projectileTransform.DOLocalMove(Vector2.zero, m_shrinkTime).SetEase(Ease.Linear).OnComplete(() => 
                {
                    projectileTransform.gameObject.SetActive(false);
                    if (!callbackUsed)
                    {
                        callbackUsed = true;
                        onClosedCallback.Invoke();
                    }
                });
            }
        }

        public override void NormalLevelUp()
        {
            base.NormalLevelUp();
            UpdateVariables(true);
            SpawnFidgetSpinners();
            SetFidgetSpinnersDamage();
        }

        public override void UltimateUpgrade()
        {
            base.UltimateUpgrade();
            DOTween.Kill(name, false);
            CloseSpinner(() =>
            {
                m_projectileList.Clear();
                transform.Clear();
                UpdateVariables(false);
                m_projectileOriginalScale = m_weaponStats.GetWeaponProjectile().transform.localScale;
                SpawnFidgetSpinners();
                SetFidgetSpinnersDamage();
                StartUltimateSpin();
            });
        }

        private void StartUltimateSpin()
        {
            OpenSpinner(() => {
                transform.DOLocalRotate(new Vector3(0, 0, m_projectileSpeed * m_duration), m_duration, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental).SetId(name);
            });
        }
    }
}