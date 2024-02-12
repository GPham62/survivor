using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using WingsMob.Survival.Global;

namespace WingsMob.Survival.Utils
{
    public static class LoadSceneUtils
    {
        public static IEnumerator IELoadScene(string sceneName, Action callback = null)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            asyncLoad.allowSceneActivation = false;
            while (asyncLoad.progress < 0.9f)
            {
                yield return null;
            }
            callback?.Invoke();
            asyncLoad.allowSceneActivation = true;
        }

        public static IEnumerator ReloadScene(Action callback = null)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
            asyncLoad.allowSceneActivation = false;
            while (asyncLoad.progress < 0.9f)
            {
                yield return null;
            }
            callback?.Invoke();
            asyncLoad.allowSceneActivation = true;
        }

        public static IEnumerator LoadHomeScene(Action callback = null)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SurvivorConfig.HomeScene, LoadSceneMode.Single);
            asyncLoad.allowSceneActivation = false;
            while (asyncLoad.progress < 0.9f)
            {
                yield return null;
            }
            callback?.Invoke();
            asyncLoad.allowSceneActivation = true;
        }
    }
}
