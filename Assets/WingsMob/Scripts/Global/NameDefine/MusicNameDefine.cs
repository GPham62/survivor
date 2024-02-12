namespace WingsMob.Survival.Global
{
    public static class MusicNameDefine
    {
        public const string MasterDir = "event:/Master/";
        public const string GamePlayDir = "event:/Gameplay/";
        public const string LevelDir = GamePlayDir + "level_";
        public const string CombatDir = GamePlayDir + "Combat/";
        public const string WeaponsDir = GamePlayDir + "Weapons/";

        public static string GetBackgroundSound() => GamePlayDir + "sfx_background";

        #region Combat
        public static string GetCombatHeal() => CombatDir + "sfx_heal";

        public static string GetCombatDiamondCollect() => CombatDir + "sfx_exp";

        public static string GetCombatPlayerDamaged() => CombatDir + "sfx_player_damaged";

        public static string GetCombatPlayerDead() => CombatDir + "sfx_player_dead";

        public static string GetCombatEnemyDead() => CombatDir + "sfx_enemy_dead";
        #endregion

        #region Weapons
        public static string GetWeaponThrow() => WeaponsDir + "sfx_throw";

        public static string GetWeaponMolotov() => WeaponsDir + "sfx_molotov";
        #endregion
    }
}