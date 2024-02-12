using DarkTonic.PoolBoss;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Global;
using WingsMob.Survival.Model;

namespace WingsMob.Survival.Combat
{
    public class Lucifer : Weapon
    {
        [SerializeField] private Transform m_droneTransform;
        [SerializeField] private Transform m_fireTargetHolder;
        [SerializeField] private Transform m_fireTarget;
        private float m_damage, m_aoe, m_speed, m_range, m_rechargeTime, m_duration, m_fireRate;
        private int m_amount;
        public override void Init(Action<string> callback)
        {
            m_tweenAttackId = null;
            m_projectileIndex = 0;
            base.Init(callback);
            UpdateVariables();
            SetupTargetPosition();
            m_weaponRecharge = m_rechargeTime;
            StartFiringSequence();
            this.RegisterListener(MethodNameDefine.OnGamePaused, OnGamePause);
            this.RegisterListener(MethodNameDefine.OnGameResumed, OnGameResume);
        }

        private void OnDisable()
        {
            this.RemoveListener(MethodNameDefine.OnGamePaused, OnGamePause);
            this.RemoveListener(MethodNameDefine.OnGameResumed, OnGameResume);
        }

        private void OnGamePause(object obj)
        {
            m_canFire = false;
            StopAllCoroutines();
        }

        private void OnGameResume(object obj)
        {
            m_canFire = true;
            StopAllCoroutines();
        }

        private void UpdateVariables()
        {
            m_damage = m_weaponStats.GetWeaponParam(WeaponParam.Damage) / 100;
            m_amount = (int)m_weaponStats.GetWeaponParam(WeaponParam.ProjectileAmount);
            m_rechargeTime = m_weaponStats.GetWeaponParam(WeaponParam.RechargeTime);
            m_aoe = m_weaponStats.GetWeaponParam(WeaponParam.AreaOfEffect) / 100;
            m_speed = m_weaponStats.GetWeaponParam(WeaponParam.ProjectileSpeed) / 100;
            m_range = m_weaponStats.GetWeaponParam(WeaponParam.Range) / 100;
            m_duration = m_weaponStats.GetWeaponParam(WeaponParam.Duration);
            m_fireRate = m_weaponStats.GetWeaponParam(WeaponParam.FireRate);
        }

        private void SetupTargetPosition()
        {
            m_fireTarget.transform.localPosition = new Vector3(0, m_range, 0);
        }

        private void ResetHolderRotation() => m_fireTargetHolder.eulerAngles = Vector3.zero;

        private bool m_canFire;
        private int m_projectileIndex;
        private float m_weaponRecharge;
        private float m_bulletRecharge;

        private void Update()
        {
            if (!m_canFire)
                return;
            m_weaponRecharge += Time.deltaTime;
            if (m_weaponRecharge >= m_rechargeTime)
            {
                StopAllCoroutines();
                m_weaponRecharge = 0f;
                StartCoroutine(SpinFireTarget());
                StartCoroutine(FireBullets());
            }
        }

        private IEnumerator SpinFireTarget()
        {
            var startRot = Quaternion.Euler(0, 0, 0);
            var endRot = Quaternion.Euler(0, 0, -180);
            float timer = m_duration;
            while (timer > 0f)
            {
                timer -= Time.deltaTime;
                m_fireTargetHolder.rotation = Quaternion.Lerp(startRot, endRot, 1 - (timer / m_duration));
                yield return null;
            }
        }

        private IEnumerator FireBullets()
        {
            float timer = m_duration;
            while (timer > 0f)
            {
                timer -= m_fireRate;
                FireBullet();
                yield return new WaitForSeconds(m_fireRate);
            }
        }

        private void FireBullet()
        {
            Transform bulletTransform = PoolBoss.Spawn(m_weaponStats.GetWeaponProjectile().transform, m_droneTransform.position, Quaternion.identity, m_levelManager.playerController.mover.transform);
            if (m_projectileIndex < m_amount / 2)
            {
                //dan bay len tren
                bulletTransform.GetComponent<LuciferProjectile>().Fly(LuciferProjectile.FlyDirection.Up,
                    m_levelManager.playerController.mover.transform.InverseTransformPoint(m_fireTarget.position),
                    m_levelManager.playerController.fighter.Damage * m_damage, m_aoe, m_speed, (m_amount - m_projectileIndex) / m_amount, true,
                    m_weaponStats.GetWeaponProjectileParticle());
            }
            if (m_projectileIndex == m_amount / 2)
            {
                //dan bay thang
                bulletTransform.GetComponent<LuciferProjectile>().Fly(LuciferProjectile.FlyDirection.Straight,
                    m_levelManager.playerController.mover.transform.InverseTransformPoint(m_fireTarget.position),
                    m_levelManager.playerController.fighter.Damage * m_damage, m_aoe, m_speed, 0f,
                    projectileParticle: m_weaponStats.GetWeaponProjectileParticle());
            }
            if (m_projectileIndex > m_amount / 2)
            {
                //dan bay xuong duoi
                bulletTransform.GetComponent<LuciferProjectile>().Fly(LuciferProjectile.FlyDirection.Down,
                    m_levelManager.playerController.mover.transform.InverseTransformPoint(m_fireTarget.position),
                    m_levelManager.playerController.fighter.Damage * m_damage, m_aoe, m_speed, (m_amount - m_projectileIndex) / m_amount, true,
                    m_weaponStats.GetWeaponProjectileParticle());
            }

            m_projectileIndex = (m_projectileIndex + 1) % m_amount;
        }

        private void StartFiringSequence()
        {
            m_canFire = true;
        }

        public override void UltimateUpgrade()
        {
            //m_levelManager.playerController.weaponManager.UnsubscribeFromTweenList(name);
        }
    }
}
