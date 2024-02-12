using WingsMob.Survival.Controller;
using WingsMob.Survival.Global;

namespace WingsMob.Survival.Combat.State
{
    public class PlayerDeathState : PlayerState
    {
        public PlayerDeathState(PlayerController player) : base(player) { }

        public override PlayerState HandleInput(PlayerController player, PlayerInput input)
        {
            if (input == PlayerInput.Idle)
            {
                return new PlayerIdleState(player);
            }
            else if (input == PlayerInput.Move)
            {
                return new PlayerMoveState(player);
            }
            return null;
        }

        public override void Act()
        {
            m_player.mover.StopMoving();
            m_player.animController.Death(m_player.transform);
            //SoundManager.Instance.PlaySound(MusicNameDefine.GetCombatPlayerDead());
            SoundManager.Instance.StopSound(MusicNameDefine.GetBackgroundSound());
            LevelManager.Instance.gameStateManager.GameLost();
        }
    }
}