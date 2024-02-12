using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Global;

namespace WingsMob.Survival.UI
{
    public class GameStatsUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_coinsCollectedText;
        [SerializeField] private TextMeshProUGUI m_killsCountText;
        [SerializeField] private TextMeshProUGUI m_gameTimeText;

        [SerializeField] private PowerBarUI powerBarUI;

        public void Init()
        {
            SetCoinCollectedText(0);
            SetKillCountText(0);
            SetGameTimeText(0);
            OnPlayerLeveledUp(1);
            powerBarUI.Reset();
        }

        private void OnEnable()
        {
            this.RegisterListener(MethodNameDefine.OnCoinsCollected, OnCoinsCollected);
            this.RegisterListener(MethodNameDefine.OnEnemyKilled, OnEnemyKilled);
            this.RegisterListener(MethodNameDefine.OnGameTimeUpdated, OnGameTimeUpdated);
            this.RegisterListener(MethodNameDefine.OnPlayerLeveledUp, OnPlayerLeveledUp);
            this.RegisterListener(MethodNameDefine.OnExpUpdated, OnExpUpdated);
            this.RegisterListener(MethodNameDefine.OnBossFightStart, ShowBossUI);
            this.RegisterListener(MethodNameDefine.OnBossFightEnd, ChangeToNormalUI);
        }

        private void OnDestroy()
        {
            this.RemoveListener(MethodNameDefine.OnCoinsCollected, OnCoinsCollected);
            this.RemoveListener(MethodNameDefine.OnEnemyKilled, OnEnemyKilled);
            this.RemoveListener(MethodNameDefine.OnGameTimeUpdated, OnGameTimeUpdated);
            this.RemoveListener(MethodNameDefine.OnPlayerLeveledUp, OnPlayerLeveledUp);
            this.RemoveListener(MethodNameDefine.OnExpUpdated, OnExpUpdated);
            this.RemoveListener(MethodNameDefine.OnBossFightStart, ShowBossUI);
            this.RemoveListener(MethodNameDefine.OnBossFightEnd, ChangeToNormalUI);
        }

        private void OnCoinsCollected(object sender = null)
        {
            SetCoinCollectedText((int)sender);
        }

        private void SetCoinCollectedText(int coinsCollected)
        {
            DOVirtual.Float(float.Parse(m_coinsCollectedText.text), coinsCollected, 0.5f, onVirtualUpdate: (float c) => {
                m_coinsCollectedText.text = ((int)c).ToString();
            });
        }

        private void OnEnemyKilled(object sender = null)
        {
            SetKillCountText((int)sender);
        }

        private void SetKillCountText(int killsCount)
        {
            m_killsCountText.text = killsCount.ToString();
        }

        private void OnGameTimeUpdated(object sender = null)
        {
            SetGameTimeText((float)sender);
        }

        private void SetGameTimeText(float timeInSeconds)
        {
            m_gameTimeText.text = TimeSpan.FromSeconds(timeInSeconds).ToString(@"mm\:ss");
        }

        private void OnPlayerLeveledUp(object sender = null)
        {
            powerBarUI.SetBarLevel((int)sender);
            powerBarUI.Reset();
        }

        private void OnExpUpdated(object sender = null)
        {
            powerBarUI.Fill((float)sender / LevelManager.Instance.gameStats.expToLevelUp);
        }

        private void ChangeToNormalUI(object obj)
        {
            powerBarUI.gameObject.SetActive(true);
        }

        private void ShowBossUI(object obj)
        {
            powerBarUI.gameObject.SetActive(false);
            m_gameTimeText.text = LevelManager.Instance.scenarioHandler.eventSpawner.currBoss.bossName;
        }
    }
}