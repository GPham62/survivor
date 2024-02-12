using DarkTonic.PoolBoss;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Combat;
using WingsMob.Survival.Environment.Collectable;
using WingsMob.Survival.Global;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Controller
{
    public class CollectableSpawner : MonoBehaviour
    {
        [SerializeField] private int m_diamondNumThreshold;
        [SerializeField] private float m_adsChestCountdownLimit;
        private float m_adsChestCountdown;
        [SerializeField] private float m_supplyBoxCountdownLimit;
        private float m_supplyBoxCountdown;
        private bool m_isBossEvent;
        private Transform m_diamondToSpawn;
        private LevelManager m_levelManager;

        private void Start()
        {
            m_adsChestCountdown = 0;
            m_isBossEvent = false;
            m_levelManager = LevelManager.Instance;
        }

        private void Update()
        {
            if (GameStatus.CurrentState != GameState.Playing)
                return;

            if (LevelManager.Instance.scenarioHandler.IsLastScenario())
                return;

            if (m_adsChestCountdown > m_adsChestCountdownLimit)
            {
                SpawnWeaponAdsChest(
                    m_levelManager.isVerticalMap ? SpawnUtils.GetRandomPosAroundPlayerVertical(m_levelManager.playerController.mover.transform.position, Random.Range(3f, 5f), m_levelManager.GetRightBorderMap()) :
                    SpawnUtils.GetRandomPosAroundPlayer(m_levelManager.playerController.mover.transform.position, Random.Range(3f, 5f)));
                m_adsChestCountdown = 0;
            }
            if (!m_isBossEvent)
                m_adsChestCountdown += Time.deltaTime;

            if (m_supplyBoxCountdown > m_supplyBoxCountdownLimit)
            {
                if (m_levelManager.isVerticalMap)
                    SpawnSupplyBox(SpawnUtils.GetRandomPosAroundPlayerVertical(m_levelManager.playerController.mover.transform.position, 6f, m_levelManager.GetRightBorderMap()));
                else
                    SpawnSupplyBox(SpawnUtils.GetRandomPosAroundPlayer(m_levelManager.playerController.mover.transform.position, 6f));
                m_supplyBoxCountdown = 0;
            }
            if (!m_isBossEvent)
                m_supplyBoxCountdown += Time.deltaTime;
        }

        private void OnEnable()
        {
            this.RegisterListener(MethodNameDefine.OnBossFightStart, OnBossFightStart);
            this.RegisterListener(MethodNameDefine.OnBossFightEnd, OnBossFightEnd);
        }

        private void OnDisable()
        {
            this.RemoveListener(MethodNameDefine.OnBossFightStart, OnBossFightStart);
            this.RemoveListener(MethodNameDefine.OnBossFightEnd, OnBossFightEnd);
        }

        private void OnBossFightEnd(object obj)
        {
            m_isBossEvent = false;
        }

        private void OnBossFightStart(object obj)
        {
            m_isBossEvent = true;
        }

        public void SpawnDiamond(Transform targetPos, float expReward)
        {
            if (PoolBoss.CategoryItemsSpawned(SurvivorConfig.CatCollectable) > m_diamondNumThreshold) return; 
            m_diamondToSpawn = GetDiamond(expReward);
            if (m_diamondToSpawn != null)
            {
                Transform diamondTransform = PoolBoss.SpawnInPool(m_diamondToSpawn, targetPos.position, targetPos.rotation);
                if (diamondTransform != null)
                    diamondTransform.GetComponent<ICollectable>().SetReward(expReward);
            }
        }

        private Transform GetDiamond(float expReward)
        {
            if (expReward >= 50)
                return GameAssets.Instance.diamondYellow;
            else if (expReward >= 20)
                return GameAssets.Instance.diamondBlue;
            else if (expReward > 0)
                return GameAssets.Instance.diamondGreen;
            else
                return null;
        }

        public void SpawnWeaponAdsChest(Vector3 targetPos, float rarity = 50)
        {
            Transform weaponAdsBoxTransform = PoolBoss.SpawnInPool(GameAssets.Instance.weaponAdsChest, targetPos, Quaternion.identity);
            weaponAdsBoxTransform.GetComponent<ICollectable>().SetReward(rarity);
        }

        public void SpawnWeaponChest(Vector3 targetPos, float rarity)
        {
            Transform weaponBoxTransform = PoolBoss.SpawnInPool(GameAssets.Instance.weaponChest, targetPos, Quaternion.identity);
            weaponBoxTransform.GetComponent<ICollectable>().SetReward(rarity);
        }

        public void SpawnSupplyBox(Vector3 targetPos)
        {
            if (m_isBossEvent) return;
            Transform supplyBoxTransform = PoolBoss.SpawnInPool(GameAssets.Instance.supplyBox, targetPos, Quaternion.identity);
        }

        public Transform SpawnMeat(Vector3 targetPos)
        {
            Transform meatTransform = PoolBoss.SpawnInPool(GameAssets.Instance.meat, targetPos, Quaternion.identity);
            if (meatTransform != null)
                meatTransform.GetComponent<ICollectable>().SetReward(18f);
            return meatTransform;
        }

        public Transform SpawnCoin(Vector3 targetPos)
        {
            return PoolBoss.SpawnInPool(GameAssets.Instance.coin, targetPos, Quaternion.identity);
        }

        public Transform SpawnCoinPack(Vector3 targetPos) => PoolBoss.SpawnInPool(GameAssets.Instance.coinPack, targetPos, Quaternion.identity);

        public Transform SpawnMagnet(Vector3 targetPos) => PoolBoss.SpawnInPool(GameAssets.Instance.magnet, targetPos, Quaternion.identity);

        public Transform SpawnBomb(Vector3 targetPos) => PoolBoss.SpawnInPool(GameAssets.Instance.bomb, targetPos, Quaternion.identity);
    }
}