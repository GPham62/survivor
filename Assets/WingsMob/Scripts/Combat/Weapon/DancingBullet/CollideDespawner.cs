using DarkTonic.PoolBoss;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WingsMob.Survival.Combat
{
    public class CollideDespawner : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            PoolBoss.Despawn(collision.transform);
        }
    }
}
