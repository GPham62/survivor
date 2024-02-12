using System;
using System.Collections;
using UnityEngine;

namespace WingsMob.Survival.Utils
{
    public static class CoroutineUtils
    {
        public static IEnumerator DelayCallback(float duration, Action callback)
        {
            yield return new WaitForSeconds(duration);
            callback.Invoke();
        }
    }
}

