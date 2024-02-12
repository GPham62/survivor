using DarkTonic.PoolBoss;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Global;
using WingsMob.Survival.Model;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Combat
{
    public class DancingBullet : Weapon
    {
        [SerializeField] private List<Transform> m_topEdges;
        [SerializeField] private List<Transform> m_bottomEdges;
        [SerializeField] private List<Transform> m_leftEdges;
        [SerializeField] private List<Transform> m_rightEdges;

        private float m_damage, m_projectileSpeed, m_projectileAmount, m_aoe;
        private int m_bounceCount;
        private List<Vector2> m_tempThrowDirs;
        private bool m_isSplittable;
        [ReadOnly] [SerializeField] private List<Transform> m_bulletList;

        public override void Init(Action<string> callback)
        {
            m_isSplittable = false;
            m_tempThrowDirs = new List<Vector2>();
            m_bulletList = new List<Transform>();
            m_cd = m_delay;
            m_canFire = true;
            base.Init(callback);
            ChangeEdgePositions();
            UpdateVariables();
            ThrowDancingBullets();
            //StartThrowingSequence();
        }

        private bool m_canFire;
        private float m_delay, m_cd;
        private void Update()
        {
            if (!m_canFire)
                return;
            if (m_bulletList.Count > 0)
                return;
            m_cd -= Time.deltaTime;
            if (m_cd < 0f)
            {
                m_cd = m_delay;
                ThrowDancingBullets();
            }
        }

        private void OnEnable()
        {
            this.RegisterListener(MethodNameDefine.OnCameraOrthoSizeChanged, ChangeEdgePositions);
            this.RegisterListener(MethodNameDefine.OnGamePaused, OnGamePause);
            this.RegisterListener(MethodNameDefine.OnGameResumed, OnGameResume);

        }

        private void OnDisable()
        {
            this.RemoveListener(MethodNameDefine.OnCameraOrthoSizeChanged, ChangeEdgePositions);
            this.RemoveListener(MethodNameDefine.OnGamePaused, OnGamePause);
            this.RemoveListener(MethodNameDefine.OnGameResumed, OnGameResume);
        }

        private void ChangeEdgePositions(object obj = null)
        {
            Vector2 bottomLeft = SpawnUtils.GetBottomLeftPos(m_levelManager.mainCam);
            Vector2 topRight = SpawnUtils.GetTopRightPos(m_levelManager.mainCam);
            m_bottomEdges[0].position = new Vector2(m_bottomEdges[0].position.x, bottomLeft.y);
            m_bottomEdges[1].position = new Vector2(m_bottomEdges[1].position.x, bottomLeft.y - 2);
            m_leftEdges[0].position = new Vector2(bottomLeft.x, m_leftEdges[0].position.y);
            m_leftEdges[1].position = new Vector2(bottomLeft.x - 2, m_leftEdges[1].position.y);
            m_topEdges[0].position = new Vector2(m_topEdges[0].position.x, topRight.y);
            m_topEdges[1].position = new Vector2(m_topEdges[1].position.x, topRight.y + 2);
            m_rightEdges[0].position = new Vector2(topRight.x, m_rightEdges[0].position.y);
            m_rightEdges[1].position = new Vector2(topRight.x + 2, m_rightEdges[1].position.y);
        }

        private void OnGamePause(object obj)
        {
            m_canFire = false;
        }

        private void OnGameResume(object obj)
        {
            m_canFire = true;
        }

        private void UpdateVariables()
        {
            m_projectileSpeed = m_weaponStats.GetWeaponParam(WeaponParam.ProjectileSpeed) / 100;
            m_damage = m_weaponStats.GetWeaponParam(WeaponParam.Damage) / 100;
            //m_rechargeTime = m_weaponStats.GetWeaponParam(WeaponParam.RechargeTime);
            m_projectileAmount = m_weaponStats.GetWeaponParam(WeaponParam.ProjectileAmount);
            m_aoe = m_weaponStats.GetWeaponParam(WeaponParam.AreaOfEffect) / 100;
            m_bounceCount = (int) m_weaponStats.GetWeaponParam(WeaponParam.ChargeCount);
        }

        //private void StartThrowingSequence()
        //{
        //    Sequence bulletThrowingSequence = DOTween.Sequence();
        //    bulletThrowingSequence.AppendCallback(() =>
        //    {
        //        ThrowDancingBullets();
        //    })
                //.AppendInterval(m_rechargeTime * (1 - m_levelManager.playerController.fighter.cooldownReduction))
        //        .SetId(name)
        //        .SetLoops(-1, LoopType.Restart);
        //}

        private void ThrowDancingBullets()
        {
            GenerateThrowDirections();
            for (int i = 0; i < m_projectileAmount; i++)
            {
                Transform bulletTransform = PoolBoss.SpawnInPool(m_weaponStats.GetWeaponProjectile().transform, new Vector2(transform.position.x + m_tempThrowDirs[i].normalized.x, transform.position.y + m_tempThrowDirs[i].normalized.y), Quaternion.identity);
                bulletTransform.GetComponent<BouncingBullet>().Init(
                    m_tempThrowDirs[i],
                    m_projectileSpeed,
                    m_levelManager.playerController.fighter.Damage * m_damage,
                    m_aoe * (1 + m_levelManager.playerController.fighter.weaponAoe),
                    m_bounceCount,
                    m_isSplittable,
                    m_isSplittable ? m_weaponStats.GetWeaponProjectile().transform : null,
                    m_weaponStats.GetWeaponProjectileParticle(),
                    (Transform transform) => m_bulletList.Add(transform),
                    (Transform transform) => m_bulletList.Remove(transform));
                m_bulletList.Add(bulletTransform);
            }
        }

        private void GenerateThrowDirections()
        {
            m_tempThrowDirs.Clear();
            m_tempThrowDirs.Add(m_levelManager.playerController.mover.GetMoveDirection());
            if (m_projectileAmount > 1)
            {
                float angleDiff = 360 / m_projectileAmount;
                for (int i = 1; i <= m_projectileAmount; i++)
                {
                    m_tempThrowDirs.Add(Quaternion.AngleAxis(angleDiff, Vector3.forward) * m_tempThrowDirs[m_tempThrowDirs.Count - 1]);
                }
            }
        }

        public override void NormalLevelUp()
        {
            base.NormalLevelUp();
            UpdateVariables();
        }

        public override void UltimateUpgrade()
        {
            base.UltimateUpgrade();
            UpdateVariables();
            m_isSplittable = true;
        }
    }
}