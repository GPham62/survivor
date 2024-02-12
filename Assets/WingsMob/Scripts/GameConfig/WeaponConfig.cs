using RotaryHeart.Lib.SerializableDictionary;
using Sirenix.OdinInspector;
using UnityEngine;
using WingsMob.Survival.Global;
using WingsMob.Survival.Model;

namespace WingsMob.Survival.GameConfig
{
    public abstract class WeaponConfig : ScriptableObject
    {
        protected const string Details = "Weapon Info/Split/Right/Details";

        [FoldoutGroup("Weapon Info"), PropertyOrder(-1)]
        [HorizontalGroup("Weapon Info/Split", Width = 100), PreviewField(100), HideLabel]
        public Sprite weaponIcon;
        [VerticalGroup("Weapon Info/Split/Right")]
        [BoxGroup(Details)] public int weaponId;
        [BoxGroup(Details)] public string weaponName;
        [BoxGroup(Details)] public bool hasProjectile;
        [BoxGroup(Details), EnableIf("hasProjectile")] public WeaponProjectile projectilePrefab;
        [BoxGroup(Details), EnableIf("hasProjectile")] public WeaponProjectileParticle projectileParticlePrefab;

        [Title("Config"), PropertyOrder(1)] 
        [SerializeField] public WeaponParamDictionary weaponParamDict;

        public float GetParam(WeaponParam param, int level)
        {
            return weaponParamDict[param].levels[level - 1];
        }

        public virtual string GetDescription(int level) => null;

        [System.Serializable]
        public class WeaponParamDictionary : SerializableDictionaryBase<WeaponParam, WeaponLevelParam> { }
        [System.Serializable]
        public class WeaponLevelParam
        {
            public float[] levels;
        }
    }
}