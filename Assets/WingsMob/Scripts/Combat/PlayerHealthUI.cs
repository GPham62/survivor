using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Global;
using WingsMob.Survival.Stats;

namespace WingsMob.Survival.Combat
{
    public class PlayerHealthUI : MonoBehaviour
    {
        private Image m_healthImg;
        
        public void Awake()
        {
            m_healthImg = GetComponent<Image>();
        }

        public void UpdateHealthUI(float hpPercentage)
        {
            m_healthImg.fillAmount = hpPercentage;
            m_healthImg.color = m_healthImg.fillAmount > 0.5f ? Color.green
                              : m_healthImg.fillAmount > 0.3f ? Color.yellow
                              : Color.red;
        }
    }
}