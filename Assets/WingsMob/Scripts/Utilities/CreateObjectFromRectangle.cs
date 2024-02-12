using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WingsMob.Survival.Utils
{
    public class CreateObjectFromRectangle : MonoBehaviour
    {
        [SerializeField] GameObject enemyPefab;
        [SerializeField] float width = 1f;
        [SerializeField] float height = 1f;
        [SerializeField] int m_numOfDotX;
        [SerializeField] int m_numOfDotY;

        [Button(ButtonSizes.Large), GUIColor(1, 0, 0)]
        public void CreateAround()
        {
            CreateEnemiesAroundPoint(transform.position);
        }

        [Button(ButtonSizes.Large), GUIColor(0, 1, 0)]
        public void DeleteChilds()
        {
            for (int i = transform.childCount; i > 0; --i)
                DestroyImmediate(transform.GetChild(0).gameObject);
        }

        public void CreateEnemiesAroundPoint(Vector3 point)
        {
            Vector3 bottomLeftPos = new Vector3(point.x - width / 2, point.y - height / 2, point.z);
            Vector3 bottomRightPos = new Vector3(point.x + width / 2, point.y - height / 2, point.z);
            Vector3 topLeftPos = new Vector3(point.x - width / 2, point.y + height / 2, point.z);
            Vector3 topRightPos = new Vector3(point.x + width / 2, point.y + height / 2, point.z);

            float topLength = topRightPos.x - topLeftPos.x;
            float bottomLength = bottomRightPos.x - bottomLeftPos.x;
            float leftLength = topLeftPos.y - bottomLeftPos.y;
            float rightLength = topRightPos.y - bottomRightPos.y;
            //Spawn at top bar
            for (int i = 0; i < m_numOfDotX; i++)
            {
                var spawnPos = new Vector3(topLength / (m_numOfDotX - 1) * i + topLeftPos.x, topLeftPos.y, point.z);
                SpawnEnemy(spawnPos);
            }

            //Spawn at bottom bar
            for (int i = 0; i < m_numOfDotX; i++)
            {
                var spawnPos = new Vector3(bottomLength / (m_numOfDotX - 1) * i + bottomLeftPos.x, bottomLeftPos.y, point.z);
                SpawnEnemy(spawnPos);
            }

            //Spawn left bar
            for (int i = 0; i < m_numOfDotY; i++)
            {
                if (i == 0 || i == m_numOfDotY - 1) continue;
                var spawnPos = new Vector3(bottomLeftPos.x, bottomLeftPos.y + leftLength / (m_numOfDotY - 1) * i, point.z);
                SpawnEnemy(spawnPos);
            }

            //Spawn right bar
            for (int i = 0; i < m_numOfDotY; i++)
            {
                if (i == 0 || i == m_numOfDotY - 1) continue;
                var spawnPos = new Vector3(bottomRightPos.x, bottomRightPos.y + rightLength / (m_numOfDotY - 1) * i, point.z);
                SpawnEnemy(spawnPos);
            }

        }

        private void SpawnEnemy(Vector3 spawnPos)
        {
            var enemy = Instantiate(enemyPefab, spawnPos, Quaternion.identity) as GameObject;
            enemy.transform.Translate(new Vector3(0, enemy.transform.localScale.y / 2, 0));
            enemy.transform.SetParent(transform);
            enemy.transform.eulerAngles = Vector3.forward * UnityEngine.Random.Range(0, 360);
        }
    }
}