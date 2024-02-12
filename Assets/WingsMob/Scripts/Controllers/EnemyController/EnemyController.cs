using UnityEngine;
using WingsMob.Survival.Combat;
using WingsMob.Survival.Combat.State;
using WingsMob.Survival.Global;
using WingsMob.Survival.Stats;

namespace WingsMob.Survival.Controller
{
    [DisallowMultipleComponent]
    public class EnemyController : MonoBehaviour
    {
        [HideInInspector] public EnemyMover mover;
        [HideInInspector] public EnemyFighter fighter;
        [HideInInspector] public BaseStats baseStats;
        public EnemyAnimationController animationController;
        public EnemyState currentState { get; protected set; }

        public virtual void Init()
        {
            mover = GetComponent<EnemyMover>();
            fighter = GetComponent<EnemyFighter>();
            baseStats = GetComponent<BaseStats>();
            baseStats.SetLevel(1);
            mover.Init(animationController, baseStats.GetBaseStat(CharacterStats.Speed));
            fighter.InitEnemy(this);
            currentState = new EnemyDeathState(this);
            animationController.Init();
        }

        protected void Awake()
        {
            Init();
        }

        public virtual void HandleInput(EnemyInput input, float? var = null)
        {
            EnemyState state = currentState.HandleInput(this, input, var);
            HandleState(state);
        }

        public virtual void HandleState(EnemyState state)
        {
            if (state != null)
            {
                currentState.Exit();
                currentState = state;
                currentState.Enter();
            }
        }
        public virtual void Act() => currentState.Act();

        //public void ChangeStats(int level)
        //{
        //    if (level == baseStats.GetLevel()) return;
        //    baseStats.SetLevel(level);
        //    fighter.UpdateStats(baseStats);
        //    mover.ChangeSpeed(baseStats.GetBaseStat(CharacterStats.Speed));

        //}

        protected virtual void OnEnable()
        {
            HandleInput(EnemyInput.FollowPlayer, null);
            animationController.Reset();
            fighter.Reset(baseStats);
        }
    }
}
