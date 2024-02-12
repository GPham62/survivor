using DarkTonic.PoolBoss;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Global;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Combat
{
    public class DamagePopup : MonoBehaviour
    {
        [Header("Color")]
        [SerializeField] private Color whiteColor;
        [SerializeField] private Color greenColor;
        [SerializeField] private Color yellowColor;
        [SerializeField] private Color orangeColor;
        [SerializeField] private Color redColor;

        [Header("Tween")]
        [SerializeField] private float moveYDist = 1;
        [SerializeField] private float timeUp;
        [SerializeField] private float timeScale;
        [SerializeField] private Ease ease;

        private TextMeshPro m_textMesh;

        private void Awake()
        {
            m_textMesh = GetComponent<TextMeshPro>();
        }

        public void Setup(int damageAmount)
        {
            TextMeshSetup(damageAmount);
            transform.localScale = Vector3.one;
            this.RegisterListener(MethodNameDefine.OnGamePaused, PauseTweenUp);
            this.RegisterListener(MethodNameDefine.OnGameResumed, ResumeTweenUp);
        }

        private void PauseTweenUp(object obj = null)
        {
            DOTween.Pause(m_seq);
        }
        private void ResumeTweenUp(object obj = null)
        {
            DOTween.TogglePause(m_seq);
        }

        private void TextMeshSetup(int damageAmount)
        {
            m_textMesh.color = damageAmount < 0 ? greenColor
                             : damageAmount < 10 ? whiteColor
                             : damageAmount < 100 ? yellowColor
                             : damageAmount < 500 ? orangeColor
                             : redColor;

            m_textMesh.SetText(Mathf.Abs(damageAmount).ToString());
            if (sortingOrder > 10000)
                sortingOrder = 0;
            sortingOrder++;
            m_textMesh.sortingOrder = sortingOrder;
        }

        private static int sortingOrder;
        private Sequence m_seq;

        public void TweenUp()
        {
            m_seq = DOTween.Sequence();
            m_seq.Append(transform.DOScale(transform.localScale, timeScale).SetEase(ease))
                .Append(transform.DOMoveY(transform.position.y + moveYDist, timeUp)
                                 .SetEase(ease))
                .OnComplete(() => 
                {
                    this.RemoveListener(MethodNameDefine.OnGamePaused, PauseTweenUp);
                    this.RemoveListener(MethodNameDefine.OnGameResumed, ResumeTweenUp);
                    PoolBoss.Despawn(transform);
                })
                .SetAutoKill(true)
                .SetRecyclable(true);
        }
    }
}