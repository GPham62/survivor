using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Model;

public class SkillCollectionConfig : MonoBehaviour
{
    [Title("Weapon Collection")]
    [AssetsOnly] public BasicWeapon[] basicWeaponCollection;
    [AssetsOnly] public Weapon[] weaponCollection;

    [Title("Weapon Augmentor Collection")]
    [AssetsOnly] public WeaponAugmentor[] weaponAugmentorCollection;

}
