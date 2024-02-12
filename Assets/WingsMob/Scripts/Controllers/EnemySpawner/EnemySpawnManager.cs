using DarkTonic.PoolBoss;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WingsMob.Survival.Environment;
using WingsMob.Survival.Global;
using WingsMob.Survival.Model;
using WingsMob.Survival.Stats;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Controller
{
    public class EnemySpawnManager : MonoBehaviour
    {
        [Title("Spawn Range")]
        [LabelText("From")][SerializeField] private float m_spawnRangeMin;
        [LabelText("To")][SerializeField] private float m_spawnRangeMax;
        private EnemiesBrainCenter m_brainCenter;
        private List<EnemyInfo> m_enemyInfosClone;
        public bool isHyperMode;

        public void Init()
        {
            m_brainCenter = LevelManager.Instance.brainCenter;
            m_spawnCoroutines = new List<Coroutine>();
            GetComponent<EnemyRelocator>().Init();
        }

        private List<Coroutine> m_spawnCoroutines;

        public void UpdateSpawnInfo(Scenario scenario)
        {
            m_enemyInfosClone = new List<EnemyInfo>(scenario.enemyInfos);
            foreach (Coroutine spawnCoroutine in m_spawnCoroutines)
                StopCoroutine(spawnCoroutine);

            m_spawnCoroutines.Clear();
            
            for (int i = 0; i < m_enemyInfosClone.Count; i++)
            {
                m_spawnCoroutines.Add(StartCoroutine(IESpawnEnemies(m_enemyInfosClone[i])));
            }
        }

        private IEnumerator IESpawnEnemies(EnemyInfo enemyInfo)
        {
            float timer = 0;
            float step = enemyInfo.spawnInterval / enemyInfo.numberOfEnemy;
            while (GameStatus.CurrentState != GameState.GameOver && GameStatus.CurrentState != GameState.GameWin)
            {
                while (timer < step)
                {
                    while (m_brainCenter.IsFull() || isPauseCoroutine || GameStatus.CurrentState != GameState.Playing)
                    {
                        yield return null;
                    }
                    timer += Time.deltaTime;
                    yield return null;
                }
                PoolEnemy(GameAssets.Instance.GetMonsterById(enemyInfo.enemyId), LevelManager.Instance.playerController.mover.transform.position, isHyperMode);
                timer = 0;
            }
           
        }

        private Transform m_enemyTransform;
        private EnemyController m_enemyControllerTemp;

        public void PoolEnemy(Transform target, Vector3 playerPos, bool isForHyperMode)
        {
            if (m_brainCenter.IsFull() || Application.targetFrameRate < 45) return;

            m_enemyTransform = (!LevelManager.Instance.isVerticalMap) ? PoolBoss.SpawnInPool(target, SpawnUtils.GetRandomEnemyPosAroundPlayer(playerPos, m_brainCenter.enemiesPos, m_spawnRangeMin, m_spawnRangeMax, isForHyperMode), target.rotation)
                : PoolBoss.SpawnInPool(target, SpawnUtils.GetRandomEnemyPosAroundPlayerVertical(playerPos, m_brainCenter.enemiesPos, LevelManager.Instance.GetRightBorderMap(), m_spawnRangeMin, m_spawnRangeMax, isForHyperMode), target.rotation);

            m_enemyControllerTemp = m_enemyTransform.GetComponent<EnemyController>();
            m_brainCenter.AddEnemyToArr(m_enemyControllerTemp);
        }

        private bool isPauseCoroutine;

        public void StopSpawnCoroutines() => isPauseCoroutine = true;

        public void ResumeSpawnCoroutines() => isPauseCoroutine = false;
    }
}
