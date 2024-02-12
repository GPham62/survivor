using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Combat;
using WingsMob.Survival.Global;

namespace WingsMob.Survival.Boss.Skill
{
    public class GroundSlam : MonoBehaviour
    {
        private float m_slamDamage;

        public void DealDamageToPlayer()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 6.025f);
            foreach (var col in colliders)
            {
                if (col.CompareTag(SurvivorConfig.PlayerTag))
                {
                    var playerFighter = col.GetComponentInParent<PlayerFighter>();
                    if (playerFighter != null)
                    {
                        playerFighter.TakeDamage(m_slamDamage);
                        return;
                    }
                }
            }
        }

        public void SetDamage(float damage)
        {
            m_slamDamage = damage;
        }
    }
}
