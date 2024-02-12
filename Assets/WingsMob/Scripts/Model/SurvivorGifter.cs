using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Model
{
    public class SurvivorGifter : SurvivorElite
    {
        [SerializeField] private PlayMakerFSM m_appearCounterFSM;

        public override void Init()
        {
            base.Init();
            m_maxHP = LevelManager.Instance.gameStats.timePlayed;
            m_currentHP = m_maxHP;
            m_controlFSM.SendEvent("START_MOVING");
        }

        protected override void PauseTime(object obj)
        {
            base.PauseTime(obj);
            m_controlFSM.enabled = false;
            m_appearCounterFSM.enabled = false;
            m_rigidBody.velocity = Vector2.zero;
        }

        protected override void ResumeTime(object obj)
        {
            base.ResumeTime(obj);
            m_controlFSM.enabled = true;
            m_appearCounterFSM.enabled = true;
            m_rigidBody.velocity = Vector2.zero;
        }

        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);
            m_controlFSM.SendEvent("TAKE_DAMAGE");
        }

        public float GetMoveSpeed() => m_speed;

        public Vector3 GetRandomPosAroundPlayer()
            => LevelManager.Instance.isVerticalMap ?
            SpawnUtils.GetRandomPosAroundPlayerVertical(m_player.position, 4f, LevelManager.Instance.GetRightBorderMap()) :
            SpawnUtils.GetRandomPosAroundPlayer(m_player.position, 4f);

        public void MoveToTarget(Vector3 target)
        {
            m_rigidBody.velocity = (target - transform.position).normalized * m_speed;
        }

        public void MoveAwayFromPlayer(float speed, Vector3 direction)
        {
            m_rigidBody.velocity = (transform.position - direction).normalized * speed;
        }

        protected override void Update()
        {
        }

        protected override void Die()
        {
            m_rigidBody.velocity = Vector2.zero;
            base.Die();
        }
    }
}