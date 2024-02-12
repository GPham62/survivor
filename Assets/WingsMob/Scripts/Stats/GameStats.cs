using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Controller;
using WingsMob.Survival.GameConfig;
using WingsMob.Survival.Global;
using WingsMob.Survival.UI;

namespace WingsMob.Survival.Stats
{
    public class GameStats : MonoBehaviour
    {
        //public float timePlayed { get; private set; }
        public float timePlayed;
        public int killsCount { get; private set; }
        public int coinsCollected { get; private set; }
        public int levelReached { get; private set; }
        public float exp { get; private set; }
        public float expToLevelUp;
        public float moneyAmp;
        public float expAmp;
        public List<float> bossKillTimes;

        public void Init()
        {
            bossKillTimes = new List<float>();
            timePlayed = exp = killsCount = coinsCollected = 0;
            moneyAmp = 0f;
            expAmp = 0f;
            levelReached = 1;
            expToLevelUp = CalculateExpToLevelUp(levelReached);
        }

        private float CalculateExpToLevelUp(int level)
        {
            return 2.8f * (level * level) + 72f * level - 25f;
        }

        private void OnEnable()
        {
            this.RegisterListener(MethodNameDefine.OnBossFightStart, OnBossFighStart);
            this.RegisterListener(MethodNameDefine.OnBossFightEnd, OnBossFighEnd);
        }

        private void OnDisable()
        {
            this.RemoveListener(MethodNameDefine.OnBossFightStart, OnBossFighStart);
            this.RemoveListener(MethodNameDefine.OnBossFightEnd, OnBossFighEnd);
        }

        private void OnBossFighStart(object obj)
        {
            m_isPauseTime = true;
        }

        private void OnBossFighEnd(object obj)
        {
            m_isPauseTime = false;
            bossKillTimes.Add(timePlayed);
        }

        private bool m_isPauseTime;

        void Update()
        {
            if (GameStatus.CurrentState != GameState.Playing)
                return;
            if (m_isPauseTime)
                return;
            timePlayed += Time.deltaTime;
            this.PostEvent(MethodNameDefine.OnGameTimeUpdated, timePlayed);
        }

        public void IncreaseGameStats(GamePlayStats gameStats, float? amount = null)
        {
            switch (gameStats)
            {
                case GamePlayStats.Coin:
                    coinsCollected = (int)(amount == null? coinsCollected + 1 : coinsCollected + amount);
                    this.PostEvent(MethodNameDefine.OnCoinsCollected, coinsCollected);
                    break;
                case GamePlayStats.KillCount:
                    killsCount = (int)(amount == null ? killsCount + 1 : killsCount + amount);
                    this.PostEvent(MethodNameDefine.OnEnemyKilled, killsCount);
                    break;
                case GamePlayStats.Exp:
                    amount = amount * (1 + expAmp/100);
                    exp = (float)(amount == null ? exp + 1 : exp + amount);
                    if (exp >= expToLevelUp)
                    {
                        LevelUp();
                    }
                    else
                        this.PostEvent(MethodNameDefine.OnExpUpdated, exp);
                    break;
            }
        }

        private void LevelUp()
        {
            if (GameStatus.CurrentState != GameState.Playing)
                return;

            levelReached++;
            expToLevelUp = CalculateExpToLevelUp(levelReached);
            this.PostEvent(MethodNameDefine.OnPlayerLeveledUp, levelReached);
            exp = 0;
            this.PostEvent(MethodNameDefine.OnExpUpdated, exp);
        }
    }
}