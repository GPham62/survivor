using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WingsMob.Survival.GameConfig
{
    [CreateAssetMenu(menuName = "Config/UltiWeaponConfig", fileName = "UltiWeaponConfig")]
    public class UltiWeaponConfig : WeaponConfig
    {
        [MultiLineProperty(10), PropertyOrder(0)] public string description;
        public override string GetDescription(int level) => description;
    }
}