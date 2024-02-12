using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WingsMob.Survival.GameConfig
{
    [CreateAssetMenu(fileName = "SupplyBoxChance", menuName = "Config/Supply Box Chance")]
    public class SupplyBoxChanceConfig : ScriptableObject
    {
        public ChanceConfig chanceConfig;
    }

    [System.Serializable]
    public class ChanceConfig
    {
        public float coinChance;
        public float coinPackChance;
        public float meatChance;
        public float bombChance;
        public float magnetChance;
    }
}