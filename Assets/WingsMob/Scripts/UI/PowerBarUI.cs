using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WingsMob.Survival.UI
{
    public class PowerBarUI : MonoBehaviour
    {
        [SerializeField] private Image m_emptyBar;
        [SerializeField] private Image m_powerBar;
        [SerializeField] private TextMeshProUGUI m_barLevel;

        public void SetBarLevel(int level) => m_barLevel.text = level.ToString();

        public void Reset()
        {
            m_powerBar.fillAmount = 0;
        }

        public void Fill(float amount)
        {
            m_powerBar.fillAmount = amount;
        }
    }
}