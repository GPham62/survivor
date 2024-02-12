using DG.Tweening;
using System;
using UnityEngine;
using WingsMob.Survival.Controller;
using WingsMob.Survival.Global;
using WingsMob.Survival.Model;

namespace WingsMob.Survival.Combat
{
    public class ElectricField : Weapon
    {
        private CircleCollider2D m_circleCollider;
        private WeaponProjectile m_electricFieldParticle;
        [SerializeField] private float m_startAoe = 260;
        public override void Init(Action<string> callback)
        {
            m_circleCollider = GetComponent<CircleCollider2D>();
            m_tweenAttackId = name;
            base.Init(callback);
            UpdateVariables();
            SpawnParticle();
            StartElectricFieldSequence();
        }

        public override void ResetStats()
        {
            DOTween.Kill(name, false);
            m_electricFieldParticle.transform.localScale = (m_aoe * (1 + m_levelManager.playerController.fighter.weaponAoe)) / m_startAoe * Vector3.one;
            StartElectricFieldSequence();
        }

        private void StartElectricFieldSequence()
        {
            Sequence electricFieldSequence = DOTween.Sequence();
            electricFieldSequence.AppendCallback(() => ElectrifyEnemies())
                                 .AppendInterval(m_rechargeTime * (1 - LevelManager.Instance.playerController.fighter.cooldownReduction))
                                 .SetId(name)
                                 .SetLoops(-1, LoopType.Restart);
        }

        private Collider2D[] m_tempCollider;
        private IDamageable m_tempEnemy;

        private void ElectrifyEnemies()
        {
            m_tempCollider = Physics2D.OverlapCircleAll(transform.position, m_circleCollider.radius * m_aoe / m_startAoe * (1 + m_levelManager.playerController.fighter.weaponAoe));
            foreach (var col in m_tempCollider)
            {
                m_tempEnemy = col.GetComponent<IDamageable>();
                if (col.isTrigger == true && m_tempEnemy != null)
                {
                    m_tempEnemy.TakeDamage(m_levelManager.playerController.fighter.Damage * m_damage);
                }
            }
        }

        private void SpawnParticle()
        {
            m_electricFieldParticle = Instantiate(m_weaponStats.GetWeaponProjectile(), transform);
        }

        private float m_damage, m_rechargeTime, m_aoe;

        private void UpdateVariables()
        {
            m_damage = m_weaponStats.GetWeaponParam(WeaponParam.Damage) / 100;
            m_rechargeTime = m_weaponStats.GetWeaponParam(WeaponParam.RechargeTime);
            m_aoe = m_weaponStats.GetWeaponParam(WeaponParam.AreaOfEffect);
        }

        public override void NormalLevelUp()
        {
            base.NormalLevelUp();
            UpdateVariables();
            m_electricFieldParticle.transform.localScale = (m_aoe * (1 + m_levelManager.playerController.fighter.weaponAoe)) / m_startAoe * Vector3.one;
        }

        public override void UltimateUpgrade()
        {
            base.UltimateUpgrade();
            UpdateVariables();
            Destroy(m_electricFieldParticle.gameObject);
            SpawnParticle();
            m_electricFieldParticle.transform.localScale = (m_aoe * (1 + m_levelManager.playerController.fighter.weaponAoe)) / m_startAoe * Vector3.one;
        }
    }
}
