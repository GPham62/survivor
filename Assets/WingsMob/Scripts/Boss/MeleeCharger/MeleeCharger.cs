using Sirenix.OdinInspector;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Boss.Skill;
using WingsMob.Survival.Global;
using WingsMob.Survival.Model;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Boss
{
    public class MeleeCharger : SurvivorBoss
    {
        [Title("Combat")]
        [SerializeField] private List<PlayMakerFSM> m_fsmToPause;
        [Title("Speed Settings")]
        [SerializeField] private float m_speedUpPercent;

        [Title("Charge Settings")]
        [SerializeField] private GameObject m_chargeWarning;
        [SerializeField] private float m_chargeRange;
        [SerializeField] private float m_chargeSpeed;
        [SerializeField] private float m_prepareChargeAnimDuration;
        [SerializeField] private float m_stunAnimDuration;

        [Title("Slam Settings")]
        [SerializeField] private GroundSlam m_groundSlam;
        [SerializeField] private float m_prepareSlamAnimation;
        [SerializeField] private float m_slamRange;
        [SerializeField] private GameObject m_slamParticle;
        [SerializeField] private float m_delaySlamParticle;
        [SerializeField] private Transform m_slamParticlePos;

        
        [TitleGroup("Animations")]
        [SpineAnimation] [SerializeField] private string m_stunAnimation;
        [TitleGroup("Animations")]
        [SpineAnimation] [SerializeField] private string m_runAnimation;
        [TitleGroup("Animations")]
        [SpineAnimation] [SerializeField] private string m_groundSlamAnimation;
        [TitleGroup("Animations")]
        [SpineAnimation] [SerializeField] private string m_prepareChargeAnimation;
        [TitleGroup("Animations")]
        [SpineAnimation] [SerializeField] private string m_chargeAnimation;

        private float m_tempChaseSpeed;

        public override void Init()
        {
            base.Init();
            m_speed = m_stats.GetBaseStat(CharacterStats.Speed);
            ResetState();
            m_groundSlam.SetDamage(m_damage * 150 / 100);
        }

        private void ResetState()
        {
            m_tempChaseSpeed = m_speed;
        }

        protected override void PauseTime(object obj)
        {
            foreach (var fsm in m_fsmToPause)
                fsm.enabled = false;
        }

        protected override void ResumeTime(object obj)
        {
            foreach (var fsm in m_fsmToPause)
                fsm.enabled = true;
        }

        public void PrepareChargeAnimation()
        {
            m_skeleton.AnimationState.SetAnimation(0, m_prepareChargeAnimation, true);
            ResetState();
            ShowChargeWarning();
        }

        private void ShowChargeWarning()
        {
            m_chargeWarning.SetActive(true);
        }

        public bool MoveToChargeRange()
        {
            MoveTowardPlayer();
            return Vector2.Distance(transform.position, m_player.position) < m_chargeRange;
        }

        public bool MoveToSlamRange()
        {
            MoveTowardPlayer();
            return Vector2.Distance(transform.position, m_player.position) < m_slamRange;
        }

        private void MoveTowardPlayer()
        {
            transform.position = Vector2.MoveTowards(transform.position, m_player.position, m_tempChaseSpeed * Time.deltaTime);
            m_tempChaseSpeed += m_speed * m_speedUpPercent * Time.deltaTime;
        }

        public float GetChargeSpeed() => m_chargeSpeed;

        private Vector2 m_chargeDirection;
        private bool m_isHittingTarget;

        public void EnableCharging()
        {
            ChargeAnimation();
            LookAtPlayer();
            m_chargeDirection = (m_player.transform.position - transform.position).normalized;
            m_isHittingTarget = false;
            m_isLookingAtPlayer = false;
        }

        public bool Charge()
        {
            transform.Translate(m_chargeSpeed * m_chargeDirection * Time.deltaTime);
            return m_isHittingTarget;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var collisionObj = collision.gameObject;
            if (collisionObj.CompareTag("MoveBlock") || collisionObj.CompareTag("Player"))
            {
                m_isHittingTarget = true;
            }
        }

        public float StunAnimation()
        {
            m_skeleton.AnimationState.SetAnimation(0, m_stunAnimation, true);
            return m_stunAnimDuration;
        }

        public float PrepareSlamAnimation()
        {
            m_skeleton.AnimationState.SetAnimation(0, m_prepareChargeAnimation, true);
            return m_prepareSlamAnimation;
        }

        public float GroundSlamAnimation()
        {
            m_skeleton.AnimationState.SetAnimation(0, m_groundSlamAnimation, false);
            return m_skeleton.Skeleton.Data.FindAnimation(m_groundSlamAnimation).Duration;
        }

        public void RunAnimation()
        {
            m_skeleton.AnimationState.SetAnimation(0, m_runAnimation, true);
        }

        public void ChargeAnimation()
        {
            m_skeleton.AnimationState.SetAnimation(0, m_chargeAnimation, true);
        }

        public void SpawnSlamParticle()
        {
            StartCoroutine(CoroutineUtils.DelayCallback(m_delaySlamParticle, () => Instantiate(m_slamParticle, m_slamParticlePos.position, m_slamParticle.transform.rotation)));
        }
    }
}
