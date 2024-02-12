using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Environment.Map
{
    public class SurvivorEndlessMap : SurvivorMap
    {
        [SerializeField] private SurvivorTile m_tilePrefab;
        private SurvivorTile[,] m_tileMaps = new SurvivorTile[3, 3];
        private float m_tileHalfSize;
        private Transform m_playerTransform;
        private float m_currentTileLeftBorder, m_currentTileRightBorder, m_currentTileBottomBorder, m_currentTileTopBorder;

        public override void SetupMap()
        {
            m_playerTransform = LevelManager.Instance.playerController.mover.transform;
            m_tileHalfSize = m_tilePrefab.GetSize() / 2;
            float startX = -m_tileHalfSize * 3;
            float startY = -m_tileHalfSize * 3;
            for (int v = 0; v < 3; v++)
            {
                for (int h = 0; h < 3; h++)
                {
                    m_tileMaps[h, v] = Instantiate(m_tilePrefab, new Vector3(transform.position.x + startX, transform.position.y + startY, transform.position.z), transform.rotation, transform);
                    m_tileMaps[h, v].AutoGenerateFoilages();
                    m_tileMaps[h, v].AutoGenerateObstacles();
                    startX += m_tileHalfSize * 2;
                }
                startX = -m_tileHalfSize * 3;
                startY += m_tileHalfSize * 2;
            }
            UpdateCurrentTileBorders();
        }

        private void Update()
        {
            UpdateTilesOnScreen();
        }

        private void UpdateCurrentTileBorders()
        {
            m_currentTileLeftBorder = m_tileMaps[1, 1].transform.position.x;
            m_currentTileRightBorder = m_tileMaps[1, 1].transform.position.x + m_tileHalfSize * 2;
            m_currentTileBottomBorder = m_tileMaps[1, 1].transform.position.y;
            m_currentTileTopBorder = m_tileMaps[1, 1].transform.position.y + m_tileHalfSize * 2;
        }

        private void UpdateTilesOnScreen()
        {
            if (m_playerTransform.position.x > m_currentTileRightBorder)
            {
                for (int v = 0; v < 3; v++)
                {
                    m_tileMaps[0, v].transform.position = new Vector3(m_tileMaps[2, v].transform.position.x + m_tileHalfSize * 2, m_tileMaps[2, v].transform.position.y, m_tileMaps[2, v].transform.position.z);
                }
                SwapCol(m_tileMaps, 0, 2);
                SwapCol(m_tileMaps, 0, 1);
                UpdateCurrentTileBorders();
            }
            else if (m_playerTransform.position.x < m_currentTileLeftBorder)
            {
                for (int v = 0; v < 3; v++)
                {
                    m_tileMaps[2, v].transform.position = new Vector3(m_tileMaps[0, v].transform.position.x - m_tileHalfSize * 2, m_tileMaps[0, v].transform.position.y, m_tileMaps[0, v].transform.position.z);
                }
                SwapCol(m_tileMaps, 0, 2);
                SwapCol(m_tileMaps, 0, 1);
                UpdateCurrentTileBorders();
            }
            else if (m_playerTransform.position.y > m_currentTileTopBorder)
            {
                for (int h = 0; h < 3; h++)
                {
                    m_tileMaps[h, 0].transform.position = new Vector3(m_tileMaps[h, 2].transform.position.x, m_tileMaps[h, 2].transform.position.y + m_tileHalfSize * 2, m_tileMaps[h, 2].transform.position.z);
                }
                SwapRow(m_tileMaps, 0, 2);
                SwapRow(m_tileMaps, 0, 1);
                UpdateCurrentTileBorders();
            }
            else if (m_playerTransform.position.y < m_currentTileBottomBorder)
            {
                for (int h = 0; h < 3; h++)
                {
                    m_tileMaps[h, 2].transform.position = new Vector3(m_tileMaps[h, 0].transform.position.x, m_tileMaps[h, 0].transform.position.y - m_tileHalfSize * 2, m_tileMaps[h, 0].transform.position.z);
                }
                SwapRow(m_tileMaps, 0, 2);
                SwapRow(m_tileMaps, 0, 1);
                UpdateCurrentTileBorders();
            }
        }

        private void SwapCol(SurvivorTile[,] a, int col0, int col1)
        {
            SurvivorTile temp = null;
            for (int i = 0; i < 3; i++)
            {
                temp = a[col0, i];
                a[col0, i] = a[col1, i];
                a[col1, i] = temp;
            }
        }

        private void SwapRow(SurvivorTile[,] a, int row0, int row1)
        {
            SurvivorTile temp = null;
            for (int i = 0; i < 3; i++)
            {
                temp = a[i, row0];
                a[i, row0] = a[i, row1];
                a[i, row1] = temp;
            }
        }
    }
}
