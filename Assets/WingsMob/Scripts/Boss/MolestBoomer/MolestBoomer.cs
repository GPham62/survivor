using DarkTonic.PoolBoss;
using DG.Tweening;
using Sirenix.OdinInspector;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Boss.Skill;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Model;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Boss
{
    public class MolestBoomer : SurvivorBoss
    {
        [Title("Combat")]
        [SerializeField] private PlayMakerFSM m_combatFSM;
        [SerializeField] private PlayMakerFSM m_moveFSM;
        [SerializeField] private bool m_isMovingAroundMap;
        [SerializeField] private float m_stunDuration;
        [SerializeField] private float m_moveSpeed = 20.0f;

        [Title("Bomb ref")]
        [SerializeField] private Transform m_bombPrefab;
        [SerializeField] private float m_throwDelay;
        [SerializeField] private Vector3 m_centerPosOffset;
        private float m_bombDamage;
        private Vector3 m_centerPos;

        [TitleGroup("Animations")]
        [SpineAnimation] [SerializeField] protected string m_digUpAnimation;
        [TitleGroup("Animations")]
        [SpineAnimation] [SerializeField] protected string m_digDownAnimation;
        [TitleGroup("Animations")]
        [SpineAnimation] [SerializeField] protected string m_attackAnimation;
        [TitleGroup("Animations")]
        [SpineAnimation] [SerializeField] protected string m_stunAnimation;

        private Quaternion m_defaultRotation;
        private Vector3 m_defaultPosition;

        private void Start()
        {
            m_defaultRotation = transform.rotation;
            m_defaultPosition = transform.position;
        }

        public override void Init()
        {
            base.Init();
            m_bombDamage = m_damage * 2.5f;
        }

        private float m_cdThrowBomb;
        protected override void Update()
        {
            if (m_isMovingAroundMap)
            {
                transform.RotateAround(m_centerPos, Vector3.forward, m_moveSpeed * Time.deltaTime);
                transform.rotation = m_defaultRotation;

                m_cdThrowBomb += Time.deltaTime;
                if (m_cdThrowBomb >= m_throwDelay)
                {
                    ThrowBombAtPlayer();
                    m_cdThrowBomb = 0;
                }
            }
        }

        private void ThrowBombAtPlayer()
        {
            Transform newBomb = PoolBoss.SpawnInPool(m_bombPrefab, transform.position, Quaternion.identity);
            newBomb.localScale = Vector2.zero;
            newBomb.DOScale(0.4f, 0.05f);
            Vector3 throwTarget = LevelManager.Instance.playerController.mover.transform.position;
            float throwTime = Vector2.Distance(throwTarget, transform.position) > 4f ? 0.5f : 0.8f;
            Ease easeY = Mathf.Abs(throwTarget.y - transform.position.y) > 4f ? Ease.InQuad : Ease.InExpo;
            newBomb.DOMoveX(throwTarget.x, throwTime).SetEase(Ease.OutQuad);
            newBomb.DOMoveY(throwTarget.y, throwTime).SetEase(easeY).OnComplete(() =>
            {
                MolestBomb molestBomb = newBomb.gameObject.GetComponent<MolestBomb>();
                molestBomb.SetDamage(m_bombDamage);
                molestBomb.TriggerExplosion();
            });

        }

        protected override void PauseTime(object obj)
        {
            m_combatFSM.enabled = false;
            m_moveFSM.enabled = false;
        }

        protected override void ResumeTime(object obj)
        {
            m_combatFSM.enabled = true;
            m_moveFSM.enabled = true;
        }

        public float DigUpAnimation()
        {
            m_skeleton.AnimationState.SetAnimation(0, m_digUpAnimation, false);
            return m_skeleton.Skeleton.Data.FindAnimation(m_digUpAnimation).Duration;
        }

        public float DigDownAnimation()
        {
            m_skeleton.AnimationState.SetAnimation(0, m_digDownAnimation, false);
            return m_skeleton.Skeleton.Data.FindAnimation(m_digDownAnimation).Duration;
        }

        public float StunAnimation()
        {
            m_skeleton.AnimationState.SetAnimation(0, m_stunAnimation, true);
            m_skeleton.AnimationState.AddAnimation(1, m_idleAnimation, true, m_stunDuration);
            return m_stunDuration;
        }

        public void AttackAnimation()
        {
            m_skeleton.AnimationState.SetAnimation(0, m_attackAnimation, true);
        }

        public void AttackThenIdleAnimation()
        {
            m_skeleton.AnimationState.SetAnimation(0, m_attackAnimation, false);
            m_skeleton.AnimationState.SetAnimation(1, m_idleAnimation, true);
        }

        public float ThrowBombPattern(MolestBombPattern pattern)
        {
            return pattern.ThrowBomb(m_bombPrefab, m_bombDamage);
        }

        public void MoveAroundMap()
        {
            m_centerPos = m_bossCageRef.transform.position + m_centerPosOffset;
            m_isMovingAroundMap = true;
        }

        public void StopMovingAroundMap() => m_isMovingAroundMap = false;

        public Vector3 GetStartPosition() => m_defaultPosition;
    }
}