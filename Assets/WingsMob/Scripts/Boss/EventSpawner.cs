using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Environment.Map;
using WingsMob.Survival.Global;
using WingsMob.Survival.Model;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Controller
{
    public class EventSpawner : MonoBehaviour
    {
        [SerializeField] private RectTransform m_bossWarning;
        [SerializeField] private float m_eventDuration = 5f;
        private Sequence m_seq;
        public SurvivorBoss currBoss;

        private void OnEnable()
        {
            this.RegisterListener(MethodNameDefine.OnGamePaused, PauseTween);
            this.RegisterListener(MethodNameDefine.OnGameResumed, ResumeTween);
        }

        private void OnDisable()
        {
            this.RemoveListener(MethodNameDefine.OnGamePaused, PauseTween);
            this.RemoveListener(MethodNameDefine.OnGameResumed, ResumeTween);
        }

        private void ResumeTween(object obj)
        {
            if (m_seq != null)
                DOTween.TogglePause(m_seq);
        }

        private void PauseTween(object obj)
        {
            if (m_seq != null)
                DOTween.Pause(m_seq);
        }

        public void SpawnBoss(EnemyInfo bossInfo)
        {
            m_bossWarning.gameObject.SetActive(true);
            float targetPos = UnityEngine.Random.Range(0, 2) == 0 ? 800f : -800f;
            m_seq = DOTween.Sequence();
            m_seq.Append(m_bossWarning.DOAnchorPosX(0, 1f).SetEase(Ease.InOutBack))
                .AppendInterval(0.2f)
                .Append(m_bossWarning.DOAnchorPosX(targetPos, 1f).OnComplete(() => { m_bossWarning.gameObject.SetActive(false); }))
                .AppendInterval(m_eventDuration-2.2f)
                .OnComplete(() => {
                    currBoss = Instantiate(GameAssets.Instance.GetBossById(bossInfo.enemyId));
                    if (LevelManager.Instance.mapManager.GetMap().GetComponent<SurvivorMap>() is SurvivorParallaxMap)
                    {
                        currBoss.isBossSpawnCenterMap = true;
                    }
                    currBoss.Init();
                    this.PostEvent(MethodNameDefine.OnBossFightStart);
                    this.PostEvent(MethodNameDefine.OnHyperModeStart);
                });
        }

        public void SpawnElite(EnemyInfo eliteInfo)
        {
            SurvivorElite newElite = LevelManager.Instance.isVerticalMap ?
                Instantiate(GameAssets.Instance.GetEliteById(eliteInfo.enemyId), SpawnUtils.GetRandomPosAroundPlayerVertical(LevelManager.Instance.playerController.mover.transform.position, 7f, LevelManager.Instance.GetRightBorderMap()), Quaternion.identity) :
                Instantiate(GameAssets.Instance.GetEliteById(eliteInfo.enemyId), SpawnUtils.GetRandomPosAroundPlayer(LevelManager.Instance.playerController.mover.transform.position, 7f), Quaternion.identity);
            newElite.Init();
        }

        public void SpawnGiftMonster(EnemyInfo gifterInfo)
        {
            SurvivorElite newGifter = LevelManager.Instance.isVerticalMap ?
                Instantiate(GameAssets.Instance.GetEliteById(gifterInfo.enemyId), SpawnUtils.GetRandomPosAroundPlayerVertical(LevelManager.Instance.playerController.mover.transform.position, 7f, LevelManager.Instance.GetRightBorderMap()), Quaternion.identity) :
                Instantiate(GameAssets.Instance.GetEliteById(gifterInfo.enemyId), SpawnUtils.GetRandomPosAroundPlayer(LevelManager.Instance.playerController.mover.transform.position, 7f), Quaternion.identity);
            newGifter.Init();
        }

        public void SpawnMonsterWave(EnemyInfo waveInfo)
        {
            for (int i = 0; i < waveInfo.numberOfEnemy; i++)
            {
                LevelManager.Instance.scenarioHandler.spawnManager.PoolEnemy(GameAssets.Instance.GetMonsterById(waveInfo.enemyId), LevelManager.Instance.playerController.mover.transform.position, true);
            }
        }
    }
}
