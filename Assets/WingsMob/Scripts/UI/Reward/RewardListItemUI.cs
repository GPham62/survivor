using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WingsMob.Survival.UI
{
    public class RewardListItemUI : MonoBehaviour
    {
        [SerializeField] private Image m_itemImage;
        [SerializeField] private Image m_iconImage;

        [SerializeField] private GameObject m_clearRewardTextObj;
        [SerializeField] private GameObject m_rewardAmountTextObj;

        [Title("Sprites")]
        [SerializeField] [PreviewField] [AssetsOnly] private Sprite m_coinSprite;
        [SerializeField] [PreviewField] [AssetsOnly] private Sprite m_expSprite;
        public enum RewardType
        {
            Coin,
            EXP
        }

        public void ChangeToClearRewardUI(float rewardAmount, RewardType rewardType)
        {
            m_rewardAmountTextObj.GetComponent<TextMeshProUGUI>().text = "+" + (int)rewardAmount;
            switch (rewardType)
            {
                case RewardType.Coin:
                    m_iconImage.sprite = m_coinSprite;
                    break;
                case RewardType.EXP:
                    m_iconImage.sprite = m_expSprite;
                    break;
            }
        }

        public void CoinReward(float rewardAmount)
        {
            m_rewardAmountTextObj.GetComponent<TextMeshProUGUI>().text = "+" + (int)rewardAmount;
            m_iconImage.sprite = m_coinSprite;
            m_clearRewardTextObj.SetActive(false);
        }

        public void EXPReward(float rewardAmount)
        {
            m_rewardAmountTextObj.GetComponent<TextMeshProUGUI>().text = "+" + (int)rewardAmount;
            m_iconImage.sprite = m_expSprite;
            m_clearRewardTextObj.SetActive(false);
        }

        public void FirstTimeEXP(float rewardAmount)
        {
            m_rewardAmountTextObj.GetComponent<TextMeshProUGUI>().text = "+" + (int)rewardAmount;
            m_iconImage.sprite = m_expSprite;
            m_clearRewardTextObj.GetComponent<TextMeshProUGUI>().text = "1st timer";
        }
        public void FirstTimeCoin(float rewardAmount)
        {
            m_rewardAmountTextObj.GetComponent<TextMeshProUGUI>().text = "+" + (int)rewardAmount;
            m_iconImage.sprite = m_coinSprite;
            m_clearRewardTextObj.GetComponent<TextMeshProUGUI>().text = "1st timer";
        }
    }
}
