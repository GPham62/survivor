using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Global;

namespace WingsMob.Survival.Boss
{
    public class ChargeWarning : MonoBehaviour
    {
        [SerializeField] private float m_scaleFactor;
        [SerializeField] private float m_chargeDuration = 2f;
        private float m_chargeStartDuration;
        public void ResetBool()
        {
            m_chargeStartDuration = 0f;
            m_isBorderReached = false;
        }

        public bool ScaleTillReachBorder()
        {
            m_chargeStartDuration += Time.deltaTime;
            if (!m_isBorderReached)
                transform.AddLocalScaleY(Time.deltaTime * m_scaleFactor);
            return m_chargeStartDuration >= m_chargeDuration && m_isBorderReached;
        }

        private bool m_isBorderReached;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("MoveBlock"))
            {
                m_isBorderReached = true;
            }
        }
    }
}
