using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Global;
using WingsMob.Survival.Stats;
using WingsMob.Survival.Utils;

namespace WingsMob.Survival.Combat
{
    public class PlayerCollideCheck : MonoBehaviour
    {
        [SerializeField] private PlayerController m_playerController;
        [SerializeField] private float m_invulnerableDuration = 0.5f;
        private bool m_isInvulnerable = false;

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag(SurvivorConfig.EnemyTag))
            {
                IAttacker enemy = collision.GetComponent<IAttacker>();
                if (enemy != null)
                {
                    if (!m_isInvulnerable)
                    {
                        m_isInvulnerable = true;
                        StartCoroutine(CoroutineUtils.DelayCallback(m_invulnerableDuration, () => m_isInvulnerable = false));
                        m_playerController.fighter.TakeDamage(enemy.GetDamage());
                    }
                    enemy.OnInteractWithTarget();
                }
            }
        }
    }
}
