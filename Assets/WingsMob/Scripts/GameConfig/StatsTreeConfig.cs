using Sirenix.OdinInspector;
using UnityEngine;
using WingsMob.Survival.Global;

namespace WingsMob.Survival.GameConfig
{
    [CreateAssetMenu(fileName = "StatsTreeConfig", menuName = "Config/StatsTree Config")]
    public class StatsTreeConfig : ScriptableObject
    {
        private const string Details = "Stats Info/Split/Right/Details";

        [FoldoutGroup("Stats Info"), PropertyOrder(-1)]
        [HorizontalGroup("Stats Info/Split", Width = 100), PreviewField(100), HideLabel]
        public Sprite statsIcon;
        [VerticalGroup("Stats Info/Split/Right")]
        [BoxGroup(Details)] public float statsAmount;
        [BoxGroup(Details)] public CharacterStats statsType;
    }
}