using Sirenix.OdinInspector;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Model;

namespace WingsMob.Survival.Boss
{
    public class ParraTree : SurvivorBoss
    {
        [Title("Combat")]
        [SerializeField] private PlayMakerFSM m_combatFSM;

        [TitleGroup("Animations")]
        [SpineAnimation] [SerializeField] private string m_shootAnimation1;
        [TitleGroup("Animations")]
        [SpineAnimation] [SerializeField] private string m_shootAnimation2;

        [Title("Shot patterns")]
        [SerializeField] private List<UbhBaseShot> m_shotPatterns;

        private GameObject m_tempParent;
        public override void Init()
        {
            base.Init();
            foreach (var shotPattern in m_shotPatterns)
            {
                m_tempParent = shotPattern.transform.parent.gameObject;
                shotPattern.SetShotCtrl(m_tempParent.GetComponent<UbhShotCtrl>());
                shotPattern.SetDamage(m_damage / 2);
                m_tempParent.SetActive(false);
            }
        }

        protected override void PauseTime(object obj)
        {
            m_combatFSM.enabled = false;
        }

        protected override void ResumeTime(object obj)
        {
            m_combatFSM.enabled = true;
        }

        public void PlayRandomShotAnimation()
        {
            string animationToPlay = Random.Range(0, 2) == 0 ? m_shootAnimation1 : m_shootAnimation2;
            m_skeleton.AnimationState.SetAnimation(0, animationToPlay, true);
        }

        public void PlayRandomShotAnimationNoLoop()
        {
            string animationToPlay = Random.Range(0, 2) == 0 ? m_shootAnimation1 : m_shootAnimation2;
            m_skeleton.AnimationState.SetAnimation(0, animationToPlay, false);
            m_skeleton.AnimationState.SetAnimation(1, m_idleAnimation, true);
        }

        public override void Die()
        {
            UbhObjectPool.instance.ReleaseAllBullet();
            base.Die();
        }
    }
}
