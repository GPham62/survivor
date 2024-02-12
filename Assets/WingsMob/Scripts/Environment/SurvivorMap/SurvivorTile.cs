using DarkTonic.PoolBoss;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Environment.Map
{
    public class SurvivorTile : MonoBehaviour
    {
        [SerializeField] private float m_size;
        [SerializeField] private List<SurvivorFoilage> m_foilages;
        [SerializeField] private List<SurvivorObstacle> m_obstacles;
        [SerializeField] private List<GameObject> borders;
        [SerializeField] private MapDirection m_direction;
        [EnableIf("m_direction", MapDirection.Vertical)]
        [SerializeField] private SurvivorObstacle m_borderLeft;
        [EnableIf("m_direction", MapDirection.Vertical)]
        [SerializeField] private SurvivorObstacle m_borderRight;
        [SerializeField] private int m_density;

        public Transform foilageHolder;

        private enum MapDirection
        {
            Horizontal,
            Vertical,
            None
        }

        private float m_spaceUsed;
        private float m_randomX, m_randomY;
        private SurvivorFoilage m_tempFoilage;
        private SurvivorObstacle m_tempObstacle;

        //[Button(ButtonSizes.Large), GUIColor(1, 0, 0)]
        //public void CreateAround()
        //{
        //    AutoGenerateFoilages();
        //    AutoGenerateObstacles();
        //}

        //[Button(ButtonSizes.Large), GUIColor(0, 1, 0)]
        //public void Clear()
        //{
        //    transform.ClearEditor();
        //}

        public float GetSize() => m_size;
        public void GenerateBorders()
        {
            switch (m_direction)
            {
                case MapDirection.Vertical:
                    SpawnBorder(m_borderLeft, new Vector3(transform.position.x, transform.position.y, transform.position.z));
                    SpawnBorder(m_borderRight, new Vector3(transform.position.x + m_size, transform.position.y, transform.position.z));
                    break;
                case MapDirection.Horizontal:
                    break;
            }
        }

        private void SpawnBorder(SurvivorObstacle border, Vector3 originalPos)
        {
            float offSetY = 0;
            for (int i = 0; i < m_size / border.Size; i++)
            {
                Instantiate(border, new Vector3(originalPos.x, originalPos.y + offSetY, originalPos.z), Quaternion.identity, transform);
                offSetY += m_borderLeft.Size;
            }
        }

        public void AutoGenerateFoilages()
        {
            for (int v = 0; v < 4; v++)
            {
                for (int i = 0; i < m_density; i++)
                {
                    RandomSpawnHorizontal(v);
                }
            }
        }

        private void RandomSpawnHorizontal(int v)
        {
            m_spaceUsed = 0;
            for (int h = 0; h < 4; h++)
            {
                m_tempFoilage = m_foilages[Random.Range(0, m_foilages.Count)];
                float spaceToSpawn = m_size / 4 * (h+1) - m_spaceUsed - m_tempFoilage.Size;
                m_randomY = m_tempFoilage.Size < m_size / 4 ? m_randomY = Random.Range(0, m_size / 4 - m_tempFoilage.Size) : 0;
                if (spaceToSpawn < 0)
                {
                    PoolBoss.Spawn(m_tempFoilage.transform,
                        new Vector3(transform.position.x + m_spaceUsed, transform.position.y + m_size / 4 * v + m_randomY, transform.position.z),
                        transform.rotation,
                        foilageHolder);
                    m_spaceUsed += m_tempFoilage.Size;
                }
                else
                {
                    m_randomX = Random.Range(0, spaceToSpawn);
                    PoolBoss.Spawn(m_tempFoilage.transform,
                        new Vector3(transform.position.x + m_spaceUsed + m_randomX, transform.position.y + m_size / 4 * v + m_randomY, transform.position.z),
                        transform.rotation,
                        foilageHolder);
                    m_spaceUsed += m_tempFoilage.Size + m_randomX;
                }
            }
        }

        public void AutoGenerateObstacles()
        {
            if (m_obstacles.Count > 0)
            {
                int numOfObstacle = Random.Range(8, 12);
                for (int i = 0; i < numOfObstacle; i++)
                {
                    m_tempObstacle = m_obstacles[Random.Range(0, m_obstacles.Count)];
                    Vector3 spawnPos = Vector3.zero;
                    bool canSpawnHere = false;
                    int safetyNet = 0;
                    while (!canSpawnHere)
                    {
                        float spawnPosX = Random.Range(transform.position.x, transform.position.x + 20.48f - m_tempObstacle.Size);
                        float spawnPosY = Random.Range(transform.position.y, transform.position.y + 20.48f - m_tempObstacle.Size);
                        spawnPos = new Vector3(spawnPosX, spawnPosY, transform.position.z);
                        canSpawnHere = PreventSpawnOverlap(spawnPos, new Vector2(m_tempObstacle.Size, m_tempObstacle.Size));
                        if (canSpawnHere) break;
                        safetyNet++;
                        if (safetyNet > 50)
                        {
                            Common.Log("Too many attempts");
                            break;
                        }
                    }
                    PoolBoss.Spawn(m_tempObstacle.transform, spawnPos, Quaternion.identity, foilageHolder);
                }
            }
        }
        public Collider2D[] colliders;

        private bool PreventSpawnOverlap(Vector3 spawnPos, Vector2 size)
        {
            colliders = Physics2D.OverlapBoxAll(transform.position, size, 0);
            for (int i = 0; i < colliders.Length; i++)
            {
                Vector3 centerPoint = colliders[i].bounds.center;
                float width = colliders[i].bounds.extents.x;
                float height = colliders[i].bounds.extents.y;
                float leftExtent = centerPoint.x - width;
                float rightExtent = centerPoint.x + width;
                float lowerExtent = centerPoint.y - height;
                float upperExtent = centerPoint.y + height;

                if (spawnPos.x >= leftExtent && spawnPos.x <= rightExtent)
                {
                    if (spawnPos.y >= lowerExtent && spawnPos.y <= upperExtent)
                        return false;
                }
            }
            return true;
        }
    }
}
