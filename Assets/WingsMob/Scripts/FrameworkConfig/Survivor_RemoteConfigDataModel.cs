using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WingsMob
{
    /// <summary>
    /// Remote config for Survivor project
    /// </summary>
    public partial class RemoteConfigDataModel
    {
        
        /// <summary>
        /// Sau bao nhieu lan upgrade skill thi co the xem ads de upgrade them
        /// </summary>
        public int RewardedBonusUpgradeCount = 3;
        public bool IsEasyMode;
        public bool IsAllLevelUnlocked;
        public float GameSpeed = 1f;
    }
}
