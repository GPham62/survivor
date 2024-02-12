using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WingsMob.Survival.UI
{
    public class CustomClickSlider : MonoBehaviour
    {
        [SerializeField] private float m_slideDuration;
        [SerializeField] private Button m_button;
        [SerializeField] private Image m_filledBG;
        [Title("References")]
        [SerializeField] private Sprite m_enabledButtonSprite;
        [SerializeField] private Sprite m_disabledButtonSprite;

        private Vector2 startPos = new Vector2(-60, 0);
        private Vector2 endPos = new Vector2(60, 0);
        private Action m_callback;
        private RectTransform m_buttonRect;
        private bool m_isActivated;

        private void Awake()
        {
            m_buttonRect = m_button.GetComponent<RectTransform>();
        }
        private void Start()
        {
            m_button.onClick.AddListener(() => ToggleState());
        }

        private void ToggleState()
        {
            m_button.interactable = false;
            if (m_isActivated)
                m_buttonRect.DOLocalMove(startPos, m_slideDuration)
                    .OnUpdate(() => 
                {
                    m_filledBG.fillAmount = (m_buttonRect.anchoredPosition.x + 60) / 120;
                }).OnComplete(() =>
                {
                    Deactivate();
                    m_button.interactable = true;
                });
            else
                m_buttonRect.DOLocalMove(endPos, m_slideDuration)
                        .OnUpdate(() =>
                        {
                            m_filledBG.fillAmount = (m_buttonRect.anchoredPosition.x + 60) / 120;
                        }).OnComplete(() =>
                        {
                            Activate();
                            m_button.interactable = true;
                        });
            m_callback?.Invoke();
        }

        public void AddOnClickEvent(Action callback)
        {
            m_callback = callback;
        }

        public void Activate()
        {
            m_isActivated = true;
            m_button.image.sprite = m_enabledButtonSprite;
            m_filledBG.fillAmount = 1;
            m_buttonRect.anchoredPosition = endPos;
        }

        public void Deactivate()
        {
            m_isActivated = false;
            m_button.image.sprite = m_disabledButtonSprite;
            m_filledBG.fillAmount = 0;
            m_buttonRect.anchoredPosition = startPos;
        }
    }
}