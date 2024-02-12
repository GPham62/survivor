using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Global;
using WingsMob.Survival.UI;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival
{
    public class GamePlayUI : MonoBehaviour
    {
        [Title("Gameplay References")]
        public GameStatsUI gameStatsUI;
        [SerializeField] private Button m_pauseButton;

        private int m_levelUpAdsCount = 0;
        private int m_levelUpAdsLimit;

        public void Init()
        {
            m_levelUpAdsLimit = GameManager.Instance.RemoteConfigData.RewardedBonusUpgradeCount;
            gameStatsUI.Init();
            m_pauseButton.onClick.AddListener(OnPauseButtonClicked);
            this.RegisterListener(MethodNameDefine.OnPlayerLeveledUp, OpenLevelUpUI);
            this.RegisterListener(MethodNameDefine.OnGameOver, OpenGameOverUI);
            this.RegisterListener(MethodNameDefine.OnGameWin, OpenGameWinUI);
        }

        private void OnPauseButtonClicked()
        {
            SoundManager.Instance.PlayButtonSound();
            SoundManager.Instance.ChangeStateSoundInstance(MusicNameDefine.GetBackgroundSound(), true);
            LevelManager.Instance.gameStateManager.PauseGame();
            UIPopUpManager.Instance.CreatePopUp<GamePauseUI>(GameAssets.Instance.pausePopupPrefab, PopUpAppearType.MoveUpToDown, keepScale: true, hasBlackBG: true);
        }

        private void OpenLevelUpUI(object sender = null)
        {
            PlayerController playerController = LevelManager.Instance.playerController;
            if (!playerController.CanLevelUp())
                return;

            LevelManager.Instance.gameStateManager.PauseGame();
            LevelUpUI levelUpUI = UIPopUpManager.Instance.CreatePopUp<LevelUpUI>(GameAssets.Instance.levelUpPopupPrefab, PopUpAppearType.ScaleSmallToBig, keepScale: true, hasBlackBG: true);
            m_levelUpAdsCount++;
            if (m_levelUpAdsCount >= m_levelUpAdsLimit)
            {
                m_levelUpAdsCount = 0;
                levelUpUI.UpdateUI(level: (int)sender, hasAds: true);
            }
            else
            {
                levelUpUI.UpdateUI(level: (int)sender, hasAds: false);
            }
            
        }

        private void OpenGameOverUI(object sender = null)
        {
            GameLoseUI gameLoseUI = UIPopUpManager.Instance.CreatePopUp<GameLoseUI>(GameAssets.Instance.losePopupPrefab, PopUpAppearType.MoveUpToDown, keepScale: true, hasBlackBG: true);
            gameLoseUI.UpdateUI();
        }

        private void OpenGameWinUI(object sender = null)
        {
            GameWinUI gameWinUI = UIPopUpManager.Instance.CreatePopUp<GameWinUI>(GameAssets.Instance.winPopupPrefab, PopUpAppearType.MoveDownToUp, keepScale: true, hasBlackBG: true);
            gameWinUI.UpdateUI();
        }

        private void OnDestroy()
        {
            this.RemoveListener(MethodNameDefine.OnPlayerLeveledUp, OpenLevelUpUI);
            this.RemoveListener(MethodNameDefine.OnGameOver, OpenGameOverUI);
            this.RemoveListener(MethodNameDefine.OnGameWin, OpenGameWinUI);
        }

        public void OpenAdsRewardUI()
        {
            GameAdsRewardUI adsRewardUI = UIPopUpManager.Instance.CreatePopUp<GameAdsRewardUI>(GameAssets.Instance.adsRewardPopupPrefab, PopUpAppearType.MoveDownToUp, keepScale: true, hasBlackBG: true);
            adsRewardUI.UpdateYellowUI();
        }

        public void OpenRewardUI()
        {
            GameAdsRewardUI adsRewardUI = UIPopUpManager.Instance.CreatePopUp<GameAdsRewardUI>(GameAssets.Instance.adsRewardPopupPrefab, PopUpAppearType.MoveDownToUp, keepScale: true, hasBlackBG: true);
            adsRewardUI.UpdatePurpleUI();
        }
    }
}
