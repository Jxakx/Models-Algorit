namespace ArenaSurvivor.Localization
{
    /// <summary>
    /// Claves de localización como constantes: el código nunca escribe un texto del juego a
    /// mano, pide una clave. Tener las claves acá (y no strings sueltos por todos lados)
    /// evita errores de tipeo que se verían recién en pantalla como una clave cruda.
    /// </summary>
    public static class LocKeys
    {
        // Panel de nivel
        public const string LevelUpTitle = "level.title";
        public const string UpgradeNewWeapon = "upgrade.new_weapon";
        public const string UpgradeWeaponLevel = "upgrade.weapon_level";
        public const string UpgradeSpeed = "upgrade.speed";
        public const string UpgradeDamage = "upgrade.damage";
        public const string UpgradeHealth = "upgrade.health";

        // Nombres de armas
        public const string WeaponMagicMissile = "weapon.magic_missile";
        public const string WeaponOrbital = "weapon.orbital";
        public const string WeaponLaser = "weapon.laser";
        public const string WeaponChain = "weapon.chain";
        public const string WeaponDrone = "weapon.drone";

        // HUD
        public const string HudLevel = "hud.level";
        public const string HudKills = "hud.kills";
        public const string HudStats = "hud.stats";
        public const string HudWeaponLevel = "hud.weapon_level";

        // Fin de partida
        public const string EndWin = "end.win";
        public const string BsodMessage = "bsod.message";

        // Pausa
        public const string PauseTitle = "pause.title";
        public const string PauseResume = "pause.resume";
        public const string PauseRestart = "pause.restart";
        public const string PauseLanguage = "pause.language";

        // Panel de Flyweight (debug)
        public const string FlyTitle = "fly.title";
        public const string FlyEnemies = "fly.enemies";
        public const string FlyProjectiles = "fly.projectiles";
        public const string FlyNumbers = "fly.numbers";
    }
}
