using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Environment.Map
{
    public class SurvivorParallaxMap : SurvivorMap
    {
        [SerializeField] private SurvivorTile m_tilePrefab;
        [ReadOnly] [SerializeField] private SurvivorTile[] m_tileMaps;
        private float m_tileHalfSize;
        private Transform m_playerTransform;
        private Vector2Int m_playerTilePosition, m_currentTilePosition;

        public override void SetupMap()
        {
            m_playerTransform = LevelManager.Instance.playerController.mover.transform;
            m_currentTilePosition = Vector2Int.zero;
            m_tileMaps = new SurvivorTile[3];
            m_tileHalfSize = m_tilePrefab.GetSize() / 2;
            float offsetX = -m_tileHalfSize;
            float offsetY = -m_tileHalfSize*3;
            for (int i = 0; i < 3; i++)
            {
                m_tileMaps[i] = Instantiate(m_tilePrefab,
                    new Vector3(m_playerTransform.position.x + offsetX, m_playerTransform.position.y + offsetY, 0), Quaternion.identity, transform);
                m_tileMaps[i].GenerateBorders();
                m_tileMaps[i].AutoGenerateFoilages();
                offsetY += m_tileHalfSize * 2;
            }
        }

        private void Update()
        {
            m_playerTilePosition.y = (int)(m_playerTransform.position.y / (m_tileHalfSize * 2));
            if (m_currentTilePosition.y != m_playerTilePosition.y)
            {
                UpdateTilesOnScreen();
                m_currentTilePosition = m_playerTilePosition;
            }
        }

        private void UpdateTilesOnScreen()
        {
            if (m_currentTilePosition.y > m_playerTilePosition.y)
            {
                //mang tile tren cung xuong duoi cung
                m_tileMaps[2].transform.position = new Vector3(m_tileMaps[0].transform.position.x, m_tileMaps[0].transform.position.y - m_tileHalfSize * 2, m_tileMaps[0].transform.position.z);

                if (UnityEngine.Random.Range(0, 2) == 1)
                {
                    m_tileMaps[2].foilageHolder.ClearBoss();
                    m_tileMaps[2].AutoGenerateFoilages();
                }
                //doi lai order trong array
                (m_tileMaps[0], m_tileMaps[1], m_tileMaps[2]) = (m_tileMaps[2], m_tileMaps[0], m_tileMaps[1]);
            }
            else
            {
                //mang tile duoi cung len tren cung
                m_tileMaps[0].transform.position = new Vector3(m_tileMaps[2].transform.position.x, m_tileMaps[2].transform.position.y + m_tileHalfSize * 2, m_tileMaps[2].transform.position.z);
                
                if (UnityEngine.Random.Range(0, 2) == 1)
                {
                    m_tileMaps[0].foilageHolder.ClearBoss();
                    m_tileMaps[0].AutoGenerateFoilages();
                }
                //doi lai order trong array
                (m_tileMaps[0], m_tileMaps[1], m_tileMaps[2]) = (m_tileMaps[1], m_tileMaps[2], m_tileMaps[0]);
            }
        }
    }
}
