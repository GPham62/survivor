using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Global;

namespace WingsMob.Survival.GameConfig
{
    [CreateAssetMenu(menuName = "Config/WeaponAugmentorConfig", fileName = "WeaponAugmentorConfig")]
    public class WeaponAugmentorConfig : ScriptableObject
    {
        [FoldoutGroup("Augmentor Info")]
        [HorizontalGroup("Augmentor Info/Split", Width = 100), PreviewField(100), HideLabel]
        public Sprite augmentorIcon;
        [VerticalGroup("Augmentor Info/Split/Right")]
        [BoxGroup("Augmentor Info/Split/Right/Details")] public int augmentorId;
        [BoxGroup("Augmentor Info/Split/Right/Details")] public string augmentorName;
        [BoxGroup("Augmentor Info/Split/Right/Details")] [TextArea] public string augmentorDescription;
        [BoxGroup("Augmentor Info/Split/Right/Details")] public int weaponId;
        [BoxGroup("Augmentor Info/Split/Right/Details")] public bool isBasicWeapon;
        public CharacterStats stats;
        public float baseValue;
        public float upgradeValue;
    }
}
