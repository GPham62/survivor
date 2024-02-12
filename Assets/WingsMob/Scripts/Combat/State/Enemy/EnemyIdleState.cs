using WingsMob.Survival.Controller;

namespace WingsMob.Survival.Combat.State
{
    public class EnemyIdleState : EnemyState
    {
        public EnemyIdleState(EnemyController enemy) : base(enemy) { }

        public override void Act()
        {
        }

        public override void Enter()
        {
            base.Enter();
            m_enemy.mover.Idle();
        }

        public override EnemyState HandleInput(EnemyController enemy, EnemyInput input, float? var)
        {
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
            else if (input == EnemyInput.Stun)
            {
                return new EnemyStunState(enemy);
            }
            return null;
        }
    }
}