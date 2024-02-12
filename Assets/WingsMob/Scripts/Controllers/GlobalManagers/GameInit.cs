using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using WingsMob.Survival.Global;
using WingsMob.Survival.UI;

namespace WingsMob.Survival
{
    [DefaultExecutionOrder(-10000)]
    public class GameInit : MonoBehaviour
    {
        [SerializeField] private LoadingBar m_loadingBar;

        /// <summary>
        /// Total time user can wait for init SDK
        /// </summary>
        private const float TIME_WAITTING = 10f;
        private const float TIME_STEP_WAITING = 0.2f;
        private float m_timeWaiting = 0f;

        private static float MIN_TIME_LOADING = 0f;

        private float m_timeLoading = 0f;

        private void Start()
        {
            GameManager.Instance.InitializeBasic();
            ProfileManager.Instance.Initialize();

            BackgroundLoadingSpeed(ThreadPriority.Normal);

            m_loadingBar.LoadTo(25f, 0.5f, StartGame);

        }

        async void StartGame()
        {
            //await IEWaitRemoteConfig();

            //if (PlayerPrefs.GetInt(WMPlayerPrefsDefine.FIRST_TIME_JOIN_GAME, 0) == 0)
            //{
            //    await IEWaitLoadSceneFirstTime();
            //}
            //else
                await IEWaitLoadScene();
        }

        IEnumerator IEWaitRemoteConfig()
        {
            //Wait remote config
            if (WMGameConfig.Instance.IsWaitRemoteConfigLoaded)
            {
                while (true)
                {
                    yield return new WaitForSeconds(TIME_STEP_WAITING);
                    m_timeWaiting += TIME_STEP_WAITING;
                    if (FirebaseManager.Instance.IsLoadRemoteSuccess)
                    {
                        Common.Log("[GameInit] Wait remote config success!");
                        yield break;
                    }
                    if (m_timeWaiting >= WMGameConfig.Instance.TimeWaitLoadRemoteConfig)
                    {
                        Common.Log("[GameInit] Wait remote config time out!");
                        yield break;
                    }
                }
            }
        }

        async Task IEWaitLoadSceneFirstTime()
        {
            AsyncOperation async = SceneManager.LoadSceneAsync(SurvivorConfig.GamePlayScene);

            async.allowSceneActivation = false;

            ProfileManager.Instance.CurrentEnergy = SurvivorConfig.MaxEnergy;
            m_loadingBar.LoadTo(60f, 0.25f);

            PlayerPrefs.SetInt(WMPlayerPrefsDefine.FIRST_TIME_JOIN_GAME, 1);

            TrackingManager.Instance.LogEvent(TrackingEventName.FIRST_GAME_OPEN);

            if (GameManager.Instance.RemoteConfigData.AppOpenEnableFirstTime)
            {
                //await IEShowAppOpenAds();
            }

            while (!async.isDone)
            {
                m_timeLoading += Time.deltaTime;
                m_loadingBar.LoadTo(60 + async.progress * 30, 0.5f);
                if (async.progress >= 0.9f && m_timeLoading >= MIN_TIME_LOADING)
                {
                    m_loadingBar.AbortAndFillFull();
                    BackgroundLoadingSpeed(ThreadPriority.Low);
                    break;
                }
                await new WaitForEndOfFrame();
            }
            PersistentUI.Instance.screenFader.FadeIn(1f, () => {
                GameStatus.CurrentState = GameState.SceneLoading;
                async.allowSceneActivation = true;
                PersistentUI.Instance.screenFader.DelayFadeOut(1.5f, 1f, () => GameStatus.CurrentState = GameState.Playing);
            });
        }


        async Task IEWaitLoadScene()
        {
            AsyncOperation async = SceneManager.LoadSceneAsync(SurvivorConfig.HomeScene);

            async.allowSceneActivation = false;

            //if (GameManager.Instance.RemoteConfigData.AppOpenEnableGameOpen)
            //{
            //    //await IEShowAppOpenAds();
            //}

            while (!async.isDone)
            {
                m_timeLoading += Time.deltaTime;
                m_loadingBar.LoadTo(60 + async.progress * 30, 0.5f);
                if (async.progress >= 0.9f)
                {
                    m_loadingBar.AbortAndFillFull();
                    BackgroundLoadingSpeed(ThreadPriority.Low);
                    async.allowSceneActivation = true;
                }
                await new WaitForEndOfFrame();
            }
        }

        IEnumerator IEShowAppOpenAds()
        {
            if (!GameManager.Instance.RemoteConfigData.AppOpenEnable)
            {
                Common.Log("[GameInit] AppOpen no need to wait");
                yield return null;
            }
            else
            {
                Common.Log("[GameInit] Wait show app open start");
                while (true)
                {
                    yield return new WaitForSeconds(TIME_STEP_WAITING);
                    m_timeWaiting += TIME_STEP_WAITING;
                    //Completed wait app open ads
                    if (AdsManager.Instance.IsAppOpenAdsLoaded)
                    {
                        Common.Log("[GameInit] Wait show app open success");
                        AdsManager.Instance.ShowAppOpenAds();
                        break;
                    }
                    if (m_timeWaiting > TIME_WAITTING)
                    {
                        Common.Log("[GameInit] Wait show app open time out!");
                        break;
                    }
                }
            }
        }

        private void BackgroundLoadingSpeed(ThreadPriority priority) => Application.backgroundLoadingPriority = priority;
    }
}