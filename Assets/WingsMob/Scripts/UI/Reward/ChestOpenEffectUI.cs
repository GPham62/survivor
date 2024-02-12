using UnityEngine;
using UnityEngine.UI;

namespace WingsMob.Survival.UI
{
    public class ChestOpenEffectUI : MonoBehaviour
    {
        [SerializeField] GameObject rewardUIContainer;
        [SerializeField] Image chestImage;

        public void StartEffect()
        {
            rewardUIContainer.SetActive(false);
            gameObject.SetActive(true);
        }

        public void AfterEffect()
        {
            rewardUIContainer.SetActive(true);
            gameObject.SetActive(false);
        }

        public void UpdateChestSprite(Sprite sprite)
        {
            chestImage.sprite = sprite;
        }
    }
}
