using UnityEngine;

namespace WingsMob.Survival.Environment.Collectable
{
    public interface IAttractable
    {
        void OnAttracted(Transform attracter);
    }
}