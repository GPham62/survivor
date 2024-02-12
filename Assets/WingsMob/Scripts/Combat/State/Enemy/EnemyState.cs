using WingsMob.Survival.Controller;

namespace WingsMob.Survival.Combat.State
{
    public abstract class EnemyState
    {
        protected EnemyController m_enemy;

        public EnemyState(EnemyController enemy)
        {
            m_enemy = enemy;
        }

        public virtual EnemyState HandleInput(EnemyController enemy, EnemyInput input, float? var) { return null; }

        public virtual void Act() { }

        public virtual void Enter() { }

        public virtual void Exit() { }

        public virtual void SetVariable(float var) { }
    }
}

