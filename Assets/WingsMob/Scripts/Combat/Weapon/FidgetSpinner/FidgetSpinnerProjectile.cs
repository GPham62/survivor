using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Global;
using System;
using WingsMob.Survival.Controller;
using DG.Tweening;
using WingsMob.Survival.Model;

namespace WingsMob.Survival.Combat
{
    public class FidgetSpinnerProjectile : WeaponProjectile
    {
        private float m_damage;

        public void Init(float damage)
        {
            SetDamage(damage);
        }

        public void SetDamage(float damage) => m_damage = damage;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.isTrigger == true)
            {
                IDamageable enemy = collision.GetComponent<IDamageable>();
                if (enemy != null)
                    enemy.TakeDamage(m_damage);
            }
        }
    }

}