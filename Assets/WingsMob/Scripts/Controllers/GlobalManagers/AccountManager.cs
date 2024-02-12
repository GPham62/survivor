using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WingsMob.Survival.Controller
{
    public class AccountManager : SingletonMono<AccountManager>
    {
        ProfileManager m_profileManager;

        public override void Init()
        {
            base.Init();
        }

        public void LevelUp()
        {
            m_profileManager.PlayerAccountExp = 0;
            m_profileManager.PlayerAccountLevel += 1;
        }

        public float GetExpAmountToLevelUp() {
            int nextLevel = m_profileManager.PlayerAccountLevel + 1;
            return nextLevel < 30 ?
                484f - 224f * nextLevel + 121f * Mathf.Pow(nextLevel, 2) + 2.6f * Mathf.Pow(nextLevel, 3)
              : 484f - 224f * 29 + 121f * Mathf.Pow(29, 2) + 2.6f * Mathf.Pow(29, 3) + 5000 * (nextLevel - 29);
        }

        public float CalculateChapterExp(int chapter, float timePlayed, bool isWin, List<float> bossKillTimes)
        {
            float expPerMin = 150 * Mathf.Pow(chapter, 0.5f);
            float expBossKill = 0;
            foreach (var bossKillTime in bossKillTimes)
                expBossKill += expPerMin * TimeSpan.FromSeconds(bossKillTime).Minutes * 0.1f;

            return isWin ?
                expPerMin + expPerMin * TimeSpan.FromSeconds(timePlayed).Minutes * 0.15f + expBossKill
                : expPerMin + expBossKill;
        }

        public float CalculateFirstTimeExp(int chapter, float timePlayed)
        {
            return 150 * Mathf.Pow(chapter, 0.5f) * TimeSpan.FromSeconds(timePlayed).Minutes * 0.3f;
        }

        public void IncreaseAccountExp(float amount)
        {
            m_profileManager.PlayerAccountExp += amount;
            if (m_profileManager.PlayerAccountExp >= GetExpAmountToLevelUp())
                LevelUp();
        }
    }
}
