using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace WingsMob.Survival.Utils
{
    public class ScreenFader : MonoBehaviour
    {
        private Image image;

        private void Start()
        {
            image = GetComponent<Image>();
            image.enabled = false;
        }

        public void FadeIn(float duration, Action callback = null)
        {
            image.enabled = true;
            image.fillAmount = 0;
            image.DOFillAmount(1, duration).SetEase(Ease.OutCirc).OnComplete(() => callback?.Invoke());
        }

        public void FadeInRadial(float duration, Action callback = null)
        {
            image.enabled = true;
            image.fillAmount = 0;
            image.fillMethod = Image.FillMethod.Radial360;
            image.DOFillAmount(1, duration).SetEase(Ease.OutCirc).OnComplete(() => callback?.Invoke());
        }

        public void FadeInHorizontal(float duration, Action callback = null)
        {
            image.enabled = true;
            image.fillAmount = 0;
            image.fillMethod = Image.FillMethod.Horizontal;
            image.DOFillAmount(1, duration).SetEase(Ease.OutCirc).OnComplete(() => callback?.Invoke());
        }

        public void DelayFadeOut(float delayTime, float duration, Action callback = null)
        {
            StartCoroutine(CoroutineUtils.DelayCallback(delayTime, () => FadeOut(duration, callback)));
        }

        public void FadeOut(float duration, Action callback = null)
        {
            image.fillAmount = 1;
            image.DOFillAmount(0, duration).SetEase(Ease.Linear).OnComplete(() =>
            {
                image.enabled = false;
                callback?.Invoke();
            });
        }

        public void FadeInThenOut(float duration, Action callback = null)
        {
            FadeIn(duration / 2, () => FadeOut(duration / 2, callback));
        }
    }
}
