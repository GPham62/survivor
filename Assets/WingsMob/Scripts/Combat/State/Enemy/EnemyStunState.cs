using WingsMob.Survival.Controller;

namespace WingsMob.Survival.Combat.State
{
    public class EnemyStunState : EnemyState
    {
        public EnemyStunState(EnemyController enemy) : base(enemy) { }

        public override void Act()
        {

        }

        private bool m_actOnce;

        public override void Enter()
        {
            base.Enter();
            m_enemy.mover.Idle();
            m_enemy.animationController.StopAnimator();
        }

        public override void Exit()
        {
            base.Exit();
            m_enemy.animationController.ResumeAnimator();

        }

        public override EnemyState HandleInput(EnemyController enemy, EnemyInput input, float? var)
        {
            if (input == EnemyInput.Idle)
            {
                return new EnemyIdleState(enemy);
            }
            if (input == EnemyInput.FollowPlayer)
            {
                return new EnemyFollowPlayerState(enemy);
            }
            if (input == EnemyInput.ShootPlayer)
            {
                return new EnemyFollowShootPlayerState(enemy);
            }
            else if (input == EnemyInput.Death)
            {
                return new EnemyDeathState(enemy);
            }
            return null;
        }
    }
}