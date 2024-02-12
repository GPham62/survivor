using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Model;

namespace WingsMob.Survival.Combat
{
    public class Meteor : WeaponProjectile
    {
        [SerializeField] private ParticleSystem m_effect;
        public void Play()
        {
            m_effect.time = 0;
            m_effect.Play();
        }
    }
}

