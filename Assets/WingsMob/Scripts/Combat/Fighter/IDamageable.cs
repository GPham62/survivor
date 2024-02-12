using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WingsMob.Survival.Combat
{
    public interface IDamageable
    {
        void TakeDamage(float damage);
        void TakeLethalDamage();
        void KnockBack(Vector2 direction, float strength, float duration);
    }
}