using Sirenix.OdinInspector;
using UnityEngine;
using WingsMob.Survival.Combat;
using WingsMob.Survival.Combat.State;
using WingsMob.Survival.Global;
using WingsMob.Survival.Stats;

namespace WingsMob.Survival.Controller
{
    public class PlayerController : MonoBehaviour
    {
        public PlayerAnimationController animController;
        public PlayerMover mover;
        public PlayerFighter fighter;
        public BaseStats playerBaseStats;
        public WeaponManager weaponManager;
        public WeaponAugmentorManager weaponAugmentorManager;
        public PlayerItemCollector itemCollector;

        private PlayerState m_currentState;

        public void Init()
        {
            mover.Init(animController, playerBaseStats.GetBaseStat(CharacterStats.Speed));
            fighter.InitPlayer(this);
            weaponManager.Init();
            weaponAugmentorManager.Init();
            m_currentState = new PlayerIdleState(this);
        }

        public void HandleInput(PlayerInput input)
        {
            if (m_currentState == null)
                return;

            PlayerState state = m_currentState.HandleInput(this, input);
            if (state != null)
            {
                m_currentState = state;
            }
            m_currentState.Enter();
        }

        public void Act() => m_currentState.Act();

        public bool CanLevelUp()
        {
            return weaponManager.CanWeaponLevelUp() || weaponAugmentorManager.CanAugmentorLevelUp();
        }

        public bool CanRewardUp()
        {
            return weaponManager.IsObtainedWeaponsUpgradable() || weaponAugmentorManager.IsObtainedAugmentorsUpgradable();
        }
    }
}