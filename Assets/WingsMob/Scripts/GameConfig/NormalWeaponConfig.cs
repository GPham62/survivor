using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Model;

namespace WingsMob.Survival.GameConfig
{
    [CreateAssetMenu(menuName = "Config/NormalWeaponConfig", fileName = "NormalWeaponConfig")]
    public class NormalWeaponConfig : WeaponConfig
    {
        [TextArea, PropertyOrder(0)] public string[] normalDescriptions;
        public override string GetDescription(int level)
            => normalDescriptions[Mathf.Clamp(level - 1, 0, normalDescriptions.Length - 1)];
    }
}