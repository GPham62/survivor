using WingsMob.Survival.Controller;
using static WingsMob.Survival.Controller.PlayerController;

namespace WingsMob.Survival.Combat.State
{
    public class PlayerIdleState : PlayerState
    {
        public PlayerIdleState(PlayerController player) : base(player){ }

        public override PlayerState HandleInput(PlayerController player, PlayerInput input) 
        {
            if (input == PlayerInput.Move)
            {
                return new PlayerMoveState(player);
            }
            else if (input == PlayerInput.Death)
            {
                return new PlayerDeathState(player);
            }
            
            return null;
        }

        public override void Enter()
        {
            m_player.mover.FreezeRigidbody();
        }

        public override void Act()
        {
            m_player.mover.StopMoving();
            m_player.animController.Idle();
        }
    }
}