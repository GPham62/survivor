using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WingsMob.Survival.UI
{
    public class ObtainedItemUI : MonoBehaviour
    {
        [SerializeField] private Image m_image;

        public void ChangeIcon(Sprite icon) => m_image.sprite = icon;

        public void Hide() => gameObject.SetActive(false);

        public void Show()
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
        }
    }
}
