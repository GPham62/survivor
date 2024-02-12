using DarkTonic.PoolBoss;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Combat;
using WingsMob.Survival.Controller;

namespace WingsMob.Survival.Environment.Collectable
{
    public class WeaponAdsChest : MonoBehaviour, ICollectable
    {
        private Collider2D m_collider;
        private LevelManager m_levelManager;

        private void Awake()
        {
            m_collider = GetComponent<Collider2D>();
        }

        public void OnCollected(Transform collector)
        {
            m_collider.enabled = false;

            if (m_levelManager.playerController.CanRewardUp())
            {
                m_levelManager.gameStateManager.PauseGame();
                m_levelManager.levelUIManager.gameplayUI.OpenAdsRewardUI();
            }

            PoolBoss.Despawn(transform);
        }

        public void SetReward(float amount)
        {
        }

        private void OnEnable()
        {
            if (m_levelManager == null)
                m_levelManager = LevelManager.Instance;
            m_collider.enabled = true;
        }
    }
}
