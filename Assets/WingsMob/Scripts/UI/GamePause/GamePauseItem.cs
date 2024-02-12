using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WingsMob.Survival.Global;

namespace WingsMob.Survival.UI
{
    public class GamePauseItem : ObtainedItemUI
    {
        [SerializeField] private Image[] m_stars;

        public void ActiveStars(int level)
        {
            for (int i = 0; i < m_stars.Length; i++)
            {
                m_stars[i].sprite = i < level ? GameAssets.Instance.starFilled : GameAssets.Instance.starEmpty;
            }
        }

        public void ActiveStarUltimate()
        {
            for (int i = 0; i < m_stars.Length; i++)
            {
                m_stars[i].sprite = GameAssets.Instance.starUltimate;
            }
        }
    }
}