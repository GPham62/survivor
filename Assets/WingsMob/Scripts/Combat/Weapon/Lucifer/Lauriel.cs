using DarkTonic.PoolBoss;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Global;
using WingsMob.Survival.Model;

namespace WingsMob.Survival.Combat
{
    public class Lauriel : Weapon
    {
        [Title("Lucifer Settings")]
        [SerializeField] private DOTweenAnimation m_flyingAnimation;
        [SerializeField] private Sprite luciferSprite;
        [SerializeField] private Transform m_extraTargetHolder;
        [SerializeField] private Transform m_extraFireTarget;
        [Title("Lauriel Settings")]
        [SerializeField] private Transform m_droneTransform;
        [SerializeField] private Transform m_fireTargetHolder;
        [SerializeField] private Transform m_fireTarget;
        private float m_damage, m_aoe, m_speed, m_range, m_rechargeTime, m_duration, m_fireRate;
        private int m_amount;
        private bool m_isUltimate;
        public override void Init(Action<string> callback)
        {
            m_tweenAttackId = null;
            m_projectileIndex = 0;
            m_isUltimate = false;
            base.Init(callback);
            UpdateVariables();
            SetupTargetPosition();
            m_weaponRecharge = m_rechargeTime * (1 - m_levelManager.playerController.fighter.cooldownReduction);
            this.RegisterListener(MethodNameDefine.OnGamePaused, OnGamePause);
            this.RegisterListener(MethodNameDefine.OnGameResumed, OnGameResume);
            m_callOnce = false;
            m_canFire = true;
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
            m_weaponRecharge = m_rechargeTime * (1 - m_levelManager.playerController.fighter.cooldownReduction);
            m_canFire = true;
            m_callOnce = false;
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
            m_extraFireTarget.transform.localPosition = new Vector3(0, m_range, 0);
        }

        private bool m_canFire;
        private int m_projectileIndex;
        private float m_weaponRecharge;
        private bool m_callOnce;

        private void Update()
        {
            if (!m_canFire)
                return;
            m_weaponRecharge += Time.deltaTime;
            if (!m_callOnce && m_weaponRecharge >= m_rechargeTime * (1 - m_levelManager.playerController.fighter.cooldownReduction))
            {
                m_callOnce = true;
                StopAllCoroutines();
                StartCoroutine(SpinTarget(m_fireTargetHolder, false));
                StartCoroutine(FireBullets(m_fireTarget));
                if (m_isUltimate)
                {
                    StartCoroutine(SpinTarget(m_extraTargetHolder, true));
                    StartCoroutine(FireBullets(m_extraFireTarget));
                }
            }
        }

        private IEnumerator SpinTarget(Transform target, bool isReverse)
        {
            float timer = m_duration * (1 + m_duration * m_levelManager.playerController.fighter.weaponDuration);
            while (timer > 0f)
            {
                timer -= Time.deltaTime;
                float rot = Mathf.Lerp(0, isReverse ? 360 : -360, 1 - (timer / (m_duration * (1 + m_duration * m_levelManager.playerController.fighter.weaponDuration))));
                target.rotation = Quaternion.Euler(0, 0, rot);
                yield return null;
            }
        }

        private IEnumerator FireBullets(Transform target)
        {
            float timer = m_duration * (1 + m_duration * m_levelManager.playerController.fighter.weaponDuration);
            float spinTime = timer;
            while (timer > 0f && spinTime > 0f)
            {
                timer -= m_fireRate;
                spinTime -= Time.deltaTime;
                FireBullet(target);
                yield return new WaitForSeconds(m_fireRate);
            }
            m_callOnce = false;
            m_weaponRecharge = 0f;
        }

        private void FireBullet(Transform target)
        {
            Transform bulletTransform = PoolBoss.Spawn(m_weaponStats.GetWeaponProjectile().transform, m_droneTransform.position, Quaternion.identity, m_levelManager.playerController.mover.transform);
            if (m_projectileIndex < m_amount / 2)
            {
                bulletTransform.GetComponent<LuciferProjectile>().Fly(
                    LuciferProjectile.FlyDirection.Up,
                    m_levelManager.playerController.mover.transform.InverseTransformPoint(target.position),
                    m_levelManager.playerController.fighter.Damage * m_damage, m_aoe * (1 + m_levelManager.playerController.fighter.weaponAoe), m_speed, (m_amount - m_projectileIndex) / m_amount,
                    projectileParticle: m_weaponStats.GetWeaponProjectileParticle());
            }
            if (m_projectileIndex == m_amount / 2)
            {
                bulletTransform.GetComponent<LuciferProjectile>().Fly(
                    LuciferProjectile.FlyDirection.Straight,
                    m_levelManager.playerController.mover.transform.InverseTransformPoint(target.position),
                    m_levelManager.playerController.fighter.Damage * m_damage, m_aoe * (1 + m_levelManager.playerController.fighter.weaponAoe), m_speed, 0f,
                    projectileParticle: m_weaponStats.GetWeaponProjectileParticle());
            }
            if (m_projectileIndex > m_amount / 2)
            {
                bulletTransform.GetComponent<LuciferProjectile>().Fly(
                    LuciferProjectile.FlyDirection.Down,
                    m_levelManager.playerController.mover.transform.InverseTransformPoint(target.position),
                    m_levelManager.playerController.fighter.Damage * m_damage, m_aoe * (1 + m_levelManager.playerController.fighter.weaponAoe), m_speed, (m_amount - m_projectileIndex) / m_amount,
                    projectileParticle: m_weaponStats.GetWeaponProjectileParticle());
            }

            m_projectileIndex = (m_projectileIndex + 1) % m_amount;
        }

        [HideInInspector] public Weapon lilith;

        public override void NormalLevelUp()
        {
            m_weaponStats.level++;
            if (m_weaponStats.level >= SurvivorConfig.MaxWeaponLevel)
            {
                lilith = m_levelManager.playerController.weaponManager.weaponsArr.FirstOrDefault(weapon => weapon != null && weapon.GetWeaponId() == 8);
                CanLevelUp = (lilith != null && !lilith.CanLevelUp) ? true : false;
            }
            UpdateVariables();
        }

        public override void UltimateUpgrade()
        {
            base.UltimateUpgrade();
            m_isUltimate = true;
            m_levelManager.playerController.weaponManager.DeleteWeapon(lilith);
            UpdateVariables();
            SetupTargetPosition();
            ChangeVisual();
        }

        private void ChangeVisual()
        {
            m_flyingAnimation.DOKill();
            m_droneTransform.AddLocalPositionX(0f);
            m_droneTransform.DOLocalMove(new Vector2(0, 0.48f), 1f).SetRelative(true).SetLoops(-1, LoopType.Yoyo);
            m_droneTransform.GetComponentInChildren<SpriteRenderer>().sprite = luciferSprite;
        }
    }
}
