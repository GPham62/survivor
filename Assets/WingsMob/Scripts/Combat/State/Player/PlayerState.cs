using WingsMob.Survival.Controller;

namespace WingsMob.Survival.Combat.State
{
    public abstract class PlayerState
    {
        protected PlayerController m_player;

        public PlayerState(PlayerController player)
        {
            m_player = player;
        }

        public virtual PlayerState HandleInput(PlayerController player, PlayerInput input)
        {
            return null;
        }

        public virtual void Act() { }

        public virtual void Enter() { }
    }
}

