using UnityEngine;
using WingsMob.Survival.Controller;

namespace WingsMob.Survival.Combat.State
{
    public class EnemyFollowShootPlayerState : EnemyState
    {
        public EnemyFollowShootPlayerState(EnemyController enemy) : base(enemy) { }

        public override EnemyState HandleInput(EnemyController enemy, EnemyInput input, float? var)
        {
            if (input == EnemyInput.Death)
            {
                return new EnemyDeathState(enemy);
            }
            else if (input == EnemyInput.Idle)
            {
                return new EnemyIdleState(enemy);
            }
            else if (input == EnemyInput.Stun)
            {
                return new EnemyStunState(enemy);
            }
            return null;
        }

        private UbhShotCtrl m_shotCtrl;
        private float m_shootRange;
        private bool m_isShooting;

        public override void Enter()
        {
            m_shotCtrl = m_enemy.GetComponent<UbhShotCtrl>();
            m_shootRange = m_enemy.baseStats.GetBaseStat(Global.CharacterStats.Range);
            m_isShooting = false;
        }

        public override void Act()
        {
            m_enemy.mover.FollowPlayer();

            if (Vector2.Distance(m_enemy.mover.transform.position, LevelManager.Instance.playerController.mover.transform.position) > m_shootRange)
            {
                m_isShooting = false;
                m_shotCtrl.StopShotRoutine();
            }

            if (m_isShooting)
                return;

            if (Vector2.Distance(m_enemy.mover.transform.position, LevelManager.Instance.playerController.mover.transform.position) < m_shootRange)
            {
                m_isShooting = true;
                m_shotCtrl.StartShotRoutine();
            }
        }
    }
}