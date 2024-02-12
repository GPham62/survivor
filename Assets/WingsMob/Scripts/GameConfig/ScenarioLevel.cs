using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Model;

namespace WingsMob.Survival.GameConfig
{
    [CreateAssetMenu(fileName = "Scenario", menuName = "Scenario/New Scenario")]
    public class ScenarioLevel : ScriptableObject
    {
        public int levelId;
        public List<Scenario> scenarios;
    }
}