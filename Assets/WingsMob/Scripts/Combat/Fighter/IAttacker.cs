using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WingsMob.Survival.Combat
{
    public interface IAttacker
    {
        float GetDamage();

        void OnInteractWithTarget();
    }
}
