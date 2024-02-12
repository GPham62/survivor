using Sirenix.OdinInspector;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Model;

namespace WingsMob.Survival.GameConfig
{
    [CreateAssetMenu(fileName = "Scenario Description", menuName = "Scenario/New Scenario Description")]
    public class LevelDescriptionConfig : ScriptableObject
    {
        public int levelId;
        public string levelChapter;
        public string levelName;
        [TextArea] public string levelDescription;
        public SkeletonDataAsset previewSpine;
    }
}