using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WingsMob.Survival.Global
{
    public static class SurvivorConfig
    {
        #region tags
        public static string PlayerTag = "Player";
        public static string EnemyTag = "Enemy";
        public static string CollectableTag = "Collectable";
        #endregion
        
        #region PoolBoss
        public static string CatCollectable = "Collectables";
        #endregion

        #region Scenes Name
        public static string LoadScene = "00_LoadScene";
        public static string HomeScene = "01_Home";
        public static string GamePlayScene = "02_GamePlay";
        #endregion

        #region weapon
        public const int MaxWeaponLevel = 5;
        public const int MaxAugmentorLevel = 5;
        public const int NumOfPlayerWeapons = 6;
        public const int NumOfAugmentor = 6;
        #endregion

        #region playerprefs
        public const string BestTime = "best_time";
        #endregion

        #region Game Infos
        public const int MaxGameLevel = 2;
        public const float MaxZValue = 5f;
        public const int LevelUpAdsLimit = 3;
        #endregion

        //#region resource_directory
        //public static string ScenarioMode = GameManager.Instance.RemoteConfigData.IsEasyMode ? "Easy" : "Hard";
        //#endregion

        #region life
        public const int MaxEnergy = 30;
        public const int SecondPerEnergy = 300;
        public static string LastEnergyCountDownUnixTimeStamp = null;
        public static int CoinsCollected = 0;
        #endregion

        #region formula
        public const float attackFactor = 0.07f;
        public const float healthFactor = 0.66f;
        #endregion
    }
}
