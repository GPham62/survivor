using WingsMob.Survival.Controller;

namespace WingsMob.Survival.Combat.State
{
    public class PlayerMoveState : PlayerState
    {
        public PlayerMoveState(PlayerController player) : base(player) { }

        public override PlayerState HandleInput(PlayerController player, PlayerInput input)
        {
            if (input == PlayerInput.Idle)
            {
                return new PlayerIdleState(player);
            }
            else if (input == PlayerInput.Death)
            {
                return new PlayerDeathState(player);
            }
            return null;
        }

        public override void Enter()
        {
            m_player.mover.UnfreezeRigidbody();
        }

        public override void Act()
        {
            m_player.mover.Move();
        }
    }
}

