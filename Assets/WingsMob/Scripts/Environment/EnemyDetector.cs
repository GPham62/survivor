using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Combat;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Model;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Environment
{
    public class EnemyDetector : MonoBehaviour
    {
        [SerializeField] private Camera m_mainCam;
        private BoxCollider2D m_boxCollider;
        private float m_closestDistance, m_farthestDistance;

        void Awake()
        {
            m_boxCollider = GetComponent<BoxCollider2D>();
            Utilities.FullScreenCollider(m_mainCam, m_boxCollider);
        }

        private EnemyController m_tempEnemyController;
        private Collider2D[] m_tempCollider;

        public int GetNumOfEnemiesOnScreen()
        {
            int numOfEnemies = 0;
            m_tempCollider = Physics2D.OverlapAreaAll(m_boxCollider.bounds.min, m_boxCollider.bounds.max);
            foreach (var col in m_tempCollider)
            {
                if (col is BoxCollider2D && col.GetComponent<EnemyController>() != null)
                {
                    numOfEnemies++;
                }
            }
            return numOfEnemies;
        }

        public List<EnemyController> GetEnemiesOnScreen()
        {
            List<EnemyController> tempEnemyList = new List<EnemyController>();
            m_tempCollider = Physics2D.OverlapAreaAll(m_boxCollider.bounds.min, m_boxCollider.bounds.max);
            foreach (var col in m_tempCollider)
            {
                m_tempEnemyController = col.GetComponent<EnemyController>();
                if (col is BoxCollider2D && m_tempEnemyController != null)
                {
                    tempEnemyList.Add(m_tempEnemyController);
                }
            }
            return tempEnemyList;
        }

        public List<Vector2> GetEnemiesPosOnScreen()
        {
            List<Vector2> tempPosList = new List<Vector2>();
            m_tempCollider = Physics2D.OverlapAreaAll(m_boxCollider.bounds.min, m_boxCollider.bounds.max);
            foreach (var col in m_tempCollider)
            {
                if (col is BoxCollider2D && col.GetComponent<EnemyController>() != null)
                {
                    tempPosList.Add(col.gameObject.transform.position);
                }
            }
            return tempPosList;
        }

        public Transform GetNearestEnemyOnScreen()
        {
            m_closestDistance = Mathf.Infinity;
            Transform target = null;
            m_tempCollider = Physics2D.OverlapAreaAll(m_boxCollider.bounds.min, m_boxCollider.bounds.max);
            for (int i = 0; i < m_tempCollider.Length; i ++)
            {
                if (m_tempCollider[i].isTrigger == true 
                    && m_tempCollider[i].GetComponent<IDamageable>() != null && m_tempCollider[i] != LevelManager.Instance.playerController.fighter)
                {
                    Transform enemyTransform = m_tempCollider[i].transform;
                    if (Vector2.Distance(transform.position, enemyTransform.position) < m_closestDistance)
                    {
                        m_closestDistance = Vector2.Distance(transform.position, enemyTransform.position);
                        target = enemyTransform;
                    }
                }
            }
            return target;
        }

        public Transform GetFarthestEnemyOnScreen()
        {
            m_farthestDistance = 0;
            Transform target = null;
            m_tempCollider = Physics2D.OverlapAreaAll(m_boxCollider.bounds.min, m_boxCollider.bounds.max);
            for (int i = 0; i < m_tempCollider.Length; i++)
            {
                if (m_tempCollider[i].isTrigger == true
                    && m_tempCollider[i].GetComponent<IDamageable>() != null && m_tempCollider[i] != LevelManager.Instance.playerController.fighter)
                {
                    Transform enemyTransform = m_tempCollider[i].transform;
                    if (Vector2.Distance(transform.position, enemyTransform.position) > m_farthestDistance)
                    {
                        m_farthestDistance = Vector2.Distance(transform.position, enemyTransform.position);
                        target = enemyTransform;
                    }
                }
            }
            return target;
        }
    }
}