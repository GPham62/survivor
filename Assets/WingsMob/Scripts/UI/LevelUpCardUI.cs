using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using WingsMob.Survival.Global;
using WingsMob.Survival.Model;
using Sirenix.OdinInspector;
using WingsMob.Survival.Controller;
using System.Linq;
using DG.Tweening;

namespace WingsMob.Survival.UI
{
    public class LevelUpCardUI : MonoBehaviour
    {
        [Title("Card References")]
        [SceneObjectsOnly] [SerializeField] private Image m_cardBackground;
        [SceneObjectsOnly] [SerializeField] private TextMeshProUGUI m_cardName;
        [SceneObjectsOnly] [SerializeField] private Image m_cardIcon;
        [SceneObjectsOnly] [SerializeField] private TextMeshProUGUI m_cardDescription;
        [SceneObjectsOnly] [SerializeField] private GameObject m_requiredWeapon;
        [SceneObjectsOnly] [SerializeField] private Image m_requiredWeaponIcon;
        [SceneObjectsOnly] [SerializeField] private Button m_adsButton;
        [SerializeField] private Image[] m_stars;
        [SerializeField] bool m_isMiniVersion;
        private GameAssets m_gameAssets;
        private Button m_button;
        private Action m_onClickCallback;
        private SkillCard m_card;
        private bool m_isForAds;

        protected virtual void Start()
        {
            m_button.onClick.AddListener(OnLevelUpCardClicked);
            m_adsButton.onClick.AddListener(OnLevelUpCardClicked);
        }

        private void OnLevelUpCardClicked()
        {
            if (m_isForAds)
                AdsManager.Instance.ViewRewardedVideo(() =>
                {
                    LevelUpCardEvent(true);
                    TrackingManager.Instance.LogEvent(TrackingEventName.VIEW_ADS_TO_SKILL_ONE);
                }, null, null, false, AdsManager.SKILL_ONE);
            else
                LevelUpCardEvent(false);
        }

        public void LevelUpCardEvent(bool hasViewedAds = false)
        {
            LevelUpUI.HasViewedAdsUpgrade = hasViewedAds;
            if (m_card is Weapon)
            {
                WeaponManager weaponManager = LevelManager.Instance.playerController.weaponManager;
                Weapon weaponToUpgrade = weaponManager.weaponsArr.FirstOrDefault(w => w != null && w.GetWeaponId() == m_card.GetId());
                if (weaponToUpgrade != null)
                    weaponToUpgrade.LevelUp();
                else
                    weaponManager.SpawnNewWeapon((Weapon)m_card);
            }
            else if (m_card is WeaponAugmentor)
            {
                WeaponAugmentorManager augmentorManager = LevelManager.Instance.playerController.weaponAugmentorManager;
                WeaponAugmentor augmentorToUpgrade = augmentorManager.weaponAugmentors.FirstOrDefault(a => a != null && a.GetAugmentorId() == m_card.GetId());
                if (augmentorToUpgrade != null)
                    augmentorToUpgrade.LevelUp();
                else
                    augmentorManager.SpawnAugmentor((WeaponAugmentor)m_card);
            }
            m_onClickCallback?.Invoke();
        }

        public void ChangeAppearanceByLevel(SkillCard skillCard, int level, Action onClickCallback = null)
        {
            if (m_gameAssets == null)
            {
                FirstInit();
            }
            ClearTempRef();
            if (m_requiredWeapon != null)
                HideRequiredWeapon();
            m_isForAds = false;
            m_adsButton.gameObject.SetActive(m_isForAds);

            if (skillCard is Weapon)
                ChangeAppearanceByLevel((Weapon)skillCard, level, onClickCallback);
            else
                ChangeAppearanceByLevel((WeaponAugmentor)skillCard, level, onClickCallback);
        }

        public void ChangeAppearanceByLevel(Weapon weapon, int level, Action onClickCallback = null)
        {
            if (level >= SurvivorConfig.MaxWeaponLevel)
            {
                m_cardBackground.sprite = m_isMiniVersion ? m_gameAssets.cardRewardUltimate : m_gameAssets.cardUltimate;
                ActiveStarsUltimate();
            }
            else
            {
                m_cardBackground.sprite = m_isMiniVersion ? m_gameAssets.cardRewardNormal : m_gameAssets.cardNormal;
                ActiveStars(level);
            }

            m_cardName.text = weapon.GetWeaponNameByLevel(level);
            m_cardIcon.sprite = weapon.GetWeaponIconByLevel(level);
            if (m_cardDescription != null)
                m_cardDescription.text = weapon.GetWeaponDescriptionByLevel(level);
            m_onClickCallback = onClickCallback;
            m_card = weapon;
        }

        public void ChangeAppearanceByLevel(WeaponAugmentor augmentor, int level, Action onClickCallback = null)
        {
            if (m_requiredWeapon != null)
                ShowRequiredWeapon(augmentor.GetUpgradableWeaponId(), augmentor.IsForBasicWeapon());
            m_cardBackground.sprite = m_isMiniVersion ? m_gameAssets.cardRewardAugmentor : m_gameAssets.cardAugmentor;
            m_cardName.text = augmentor.GetAugmentorName();
            m_cardIcon.sprite = augmentor.GetAugmentorIcon();
            if (m_cardDescription != null)
                m_cardDescription.text = augmentor.GetAugmentorDescription();
            ActiveStars(level);

            m_onClickCallback = onClickCallback;
            m_card = augmentor;
        }

        public void ChangeAppearance(SkillCard skillCard, bool isForAds, Action onClickCallback = null)
        {
            if (m_gameAssets == null)
            {
                FirstInit();
            }
            ClearTempRef();
            if (m_requiredWeapon != null)
                HideRequiredWeapon();
            m_isForAds = isForAds;
            m_adsButton.gameObject.SetActive(isForAds);

            if (skillCard is Weapon)
                ChangeAppearance((Weapon)skillCard, onClickCallback);
            else
                ChangeAppearance((WeaponAugmentor)skillCard, onClickCallback);
        }

        private void HideRequiredWeapon()
        {
            m_requiredWeapon.SetActive(false);
        }

        public void ChangeAppearance(Weapon weapon, Action onClickCallback = null)
        {
            //for lilith & lauriel
            if (weapon.GetWeaponId() == 7 && !weapon.IsWeaponPreviewUltimate())
            {
                ShowRequiredWeapon(8, false);
            }
            else if (weapon.GetWeaponId() == 8)
            {
                ShowRequiredWeapon(7, false);
            }

            if (weapon.IsWeaponPreviewUltimate())
            {
                m_cardBackground.sprite = m_isMiniVersion ? m_gameAssets.cardRewardUltimate : m_gameAssets.cardUltimate;
                ActiveStarsUltimate();
            }
            else
            {
                m_cardBackground.sprite = m_isMiniVersion ? m_gameAssets.cardRewardNormal : m_gameAssets.cardNormal;
                ActiveStars(weapon.GetWeaponLevel());
            }

            m_cardName.text = weapon.GetWeaponPreviewName();
            m_cardIcon.sprite = weapon.GetWeaponPreviewIcon();
            if (m_cardDescription != null)
                m_cardDescription.text = weapon.GetWeaponPreviewDescription();
            m_onClickCallback = onClickCallback;
            m_card = weapon;
        }

        public void ChangeAppearance(WeaponAugmentor augmentor, Action onClickCallback = null)
        {
            if (!m_isForAds && m_requiredWeapon != null)
                ShowRequiredWeapon(augmentor.GetUpgradableWeaponId(), augmentor.IsForBasicWeapon());
            m_cardBackground.sprite = m_isMiniVersion ? m_gameAssets.cardRewardAugmentor : m_gameAssets.cardAugmentor;
            m_cardName.text = augmentor.GetAugmentorName();
            m_cardIcon.sprite = augmentor.GetAugmentorIcon();
            if (m_cardDescription != null)
                m_cardDescription.text = augmentor.GetAugmentorDescription();
            ActiveStars(augmentor.GetAugmentorLevel());

            m_onClickCallback = onClickCallback;
            m_card = augmentor;
        }

        private void ShowRequiredWeapon(int id, bool isBasicWeapon)
        {
            m_requiredWeapon.SetActive(true);
            m_requiredWeaponIcon.sprite = isBasicWeapon ?
                m_gameAssets.GetBasicWeaponById(id).GetWeaponPreviewIcon() :
                m_gameAssets.GetWeaponById(id).GetWeaponPreviewIcon();
        }

        private void ClearTempRef()
        {
            m_onClickCallback = null;
            m_card = null;
            m_starTweens.Clear();
        }

        public void Show() => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);

        private void FirstInit()
        {
            m_gameAssets = GameAssets.Instance;
            m_button = GetComponent<Button>();
            m_starTweens = new List<Tween>();
        }

        private List<Tween> m_starTweens;

        private void ActiveStars(int level)
        {
            for (int i = 0; i < m_stars.Length; i++)
            {
                m_stars[i].sprite = i < level + 1 ? m_gameAssets.starFilled : m_gameAssets.starEmpty;
                m_stars[i].color = new Color(m_stars[i].color.r, m_stars[i].color.g, m_stars[i].color.b, 1f);
            }
            m_starTweens.Add(m_stars[level].DOFade(0.3f, 0.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).OnKill(() =>
            {
                m_stars[level].color = new Color(m_stars[level].color.r, m_stars[level].color.g, m_stars[level].color.b, 1f);
            }));
        }

        private void ActiveStarsUltimate()
        {
            for (int i = 0; i < m_stars.Length; i++)
            {
                m_stars[i].sprite = m_gameAssets.starUltimate;
                m_starTweens.Add(m_stars[i].DOFade(0.3f, 0.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo));
            }
        }

        private void OnDisable()
        {
            foreach (var starTween in m_starTweens)
            {
                starTween.Kill();
            }
        }

        public SkillCard GetCardInfo() => m_card;
    }
}