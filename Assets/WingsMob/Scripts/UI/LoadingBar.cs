using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WingsMob.Survival.UI
{
    public class LoadingBar : MonoBehaviour
    {
        [SerializeField] Image m_imgLoadingPercent;
        [SerializeField] TextMeshProUGUI m_txtLoadingPercent;

        private float m_currentPercent;

        public void LoadTo(float percent, float duration, Action callback = null)
        {
            if (percent > 100) percent = 100;
            if (percent < 0) percent = 0;

            DOTween.KillAll(false);

            TweenFillBar(percent, duration);

            TweenText(percent, duration, callback);
        }

        public void AbortAndFillFull()
        {
            DOTween.Kill(m_imgLoadingPercent);
            m_imgLoadingPercent.fillAmount = 1f;
        }

        private void TweenFillBar(float percent, float duration)
        {
            m_imgLoadingPercent.DOFillAmount(percent / 100, duration);
        }

        private void TweenText(float percent, float duration, Action callback)
        {
            DOVirtual.Float(m_currentPercent, percent, duration, onVirtualUpdate: (float p) => {
                m_currentPercent = p;
                SetText((int)p);
            }).OnComplete(() => callback?.Invoke());
        }

        private void SetText(int percent) => m_txtLoadingPercent.text = percent + "%";
    }
}
