using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Environment.Collectable;
using WingsMob.Survival.Global;

namespace WingsMob.Survival.Combat
{
    public class PlayerItemCollector : MonoBehaviour
    {
        [SerializeField] private CircleCollider2D m_collider;
        private float m_startRadius;

        private void Start()
        {
            m_startRadius = m_collider.radius;    
        }

        public void IncreaseSize(float size)
        {
            m_collider.radius = m_startRadius + m_startRadius * size / 100;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(SurvivorConfig.CollectableTag))
            {
                other.GetComponent<ICollectable>().OnCollected(LevelManager.Instance.playerController.mover.transform);
            }
        }
    }
}