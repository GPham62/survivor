using DarkTonic.PoolBoss;
using UnityEngine;

namespace WingsMob.Survival.Combat
{
    public class CustomEnemyBullet : UbhBullet, IAttacker
    {
        public float GetDamage()
        {
            return m_damage;
        }

        public void OnInteractWithTarget()
        {
            UbhObjectPool.instance.ReleaseBullet(this);
        }
    }
}