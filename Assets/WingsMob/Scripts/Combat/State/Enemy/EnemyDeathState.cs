using UnityEngine;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Global;

namespace WingsMob.Survival.Combat.State
{
    public class EnemyDeathState : EnemyState
    {
        public EnemyDeathState(EnemyController enemy) : base(enemy)
        {
        }

        public override EnemyState HandleInput(EnemyController enemy, EnemyInput input, float? var)
        {
            if (input == EnemyInput.FollowPlayer)
            {
                return new EnemyFollowPlayerState(enemy);
            }
            else if (input == EnemyInput.ShootPlayer)
            {
                return new EnemyFollowShootPlayerState(enemy);
            }
            else if (input == EnemyInput.Idle)
            {
                return new EnemyIdleState(enemy);
            }
            return null;
        }

        private LevelManager m_levelManager;

        public override void Enter()
        {
            m_levelManager = LevelManager.Instance;
            m_enemy.mover.StopMoving();
            SpawnCollectables();
            m_enemy.animationController.Death(m_enemy.transform, () => m_levelManager.brainCenter.RemoveEnemyFromArr(m_enemy));
        }

        public override void Act()
        {
        }

        private void SpawnCollectables()
        {
            if (Random.Range(0, 1f) <= m_enemy.baseStats.GetBaseStat(CharacterStats.EXPDropChance))
            {
                m_levelManager.collectableSpawner.SpawnDiamond(m_enemy.transform, m_enemy.baseStats.GetBaseStat(CharacterStats.ExperienceReward));
            }
        }
    }
}
