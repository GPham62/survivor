using UnityEngine;

namespace WingsMob.Survival.Environment.Collectable
{
    public interface ICollectable
    {
        void SetReward(float amount);
        void OnCollected(Transform collector);
    }
}