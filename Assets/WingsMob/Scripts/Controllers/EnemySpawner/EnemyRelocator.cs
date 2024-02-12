using DarkTonic.PoolBoss;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WingsMob.Survival.Environment;
using WingsMob.Survival.Global;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Controller
{
    public class EnemyRelocator : MonoBehaviour
    {
        [SerializeField] float m_distanceToRelocate = 5f;
        private Transform m_playerTransform;
        private Camera m_mainCam;
        private EnemiesBrainCenter m_brainCenter;

        public void Init()
        {
            m_playerTransform = LevelManager.Instance.playerController.mover.transform;
            m_mainCam = LevelManager.Instance.mainCam;
            m_brainCenter = LevelManager.Instance.brainCenter;
        }

        private void Update()
        {
            if (GameStatus.CurrentState != GameState.Playing)
                return;
            for (int i = 0; i < m_brainCenter.enemiesPos.Length; i++)
            {
                if (m_brainCenter.enemiesArr[i] != null)
                {
                    if (Vector2.Distance(m_brainCenter.enemiesPos[i], m_playerTransform.position) > m_distanceToRelocate)
                    {

                        m_brainCenter.enemiesArr[i].transform.position = LevelManager.Instance.isVerticalMap ?
                            SpawnUtils.GetRandomPosWithSpawnDirectionVertical(
                                 mainCam: m_mainCam,
                                    spawnDirection: SpawnUtils.GetOppositeSpawnDirection(
                                        SpawnUtils.GetEnemySpawnDirection(m_playerTransform, m_brainCenter.enemiesArr[i].transform.transform, m_mainCam)),
                                    rightBorderX: (float)LevelManager.Instance.GetRightBorderMap())
                            : SpawnUtils.GetRandomPosWithSpawnDirection(
                                    mainCam: m_mainCam,
                                    spawnDirection: SpawnUtils.GetOppositeSpawnDirection(
                                        SpawnUtils.GetEnemySpawnDirection(m_playerTransform, m_brainCenter.enemiesArr[i].transform.transform, m_mainCam)));
                    }
                }
            }
        }
    }
}