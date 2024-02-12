using WingsMob.Survival.Controller;

namespace WingsMob.Survival.Combat.State
{
    public class EnemyFollowPlayerState : EnemyState
    {
        public EnemyFollowPlayerState(EnemyController enemy) : base(enemy) { }

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

        public override void Act()
        {
            m_enemy.mover.FollowPlayer();
        }
    }
}