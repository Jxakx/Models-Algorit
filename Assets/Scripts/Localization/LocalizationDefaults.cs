namespace ArenaSurvivor.Localization
{
    /// <summary>
    /// Los textos del juego en cada idioma. LocalizationTable los agrega al asset (menú
    /// contextual "Completar con textos por defecto"), y desde ahí se pueden retocar en el
    /// Inspector sin tocar código — esta lista solo siembra las claves que falten.
    /// </summary>
    public static class LocalizationDefaults
    {
        public static readonly (string key, string spanish, string english)[] All =
        {
            (LocKeys.LevelUpTitle, "¡Subiste de nivel!", "Level up!"),
            (LocKeys.UpgradeNewWeapon, "Nueva arma: {0}", "New weapon: {0}"),
            (LocKeys.UpgradeWeaponLevel, "{0} Nv.{1}", "{0} Lv.{1}"),
            (LocKeys.UpgradeSpeed, "Velocidad +10%", "Speed +10%"),
            (LocKeys.UpgradeDamage, "Daño +10%", "Damage +10%"),
            (LocKeys.UpgradeHealth, "Vida máxima +20", "Max health +20"),

            (LocKeys.WeaponMagicMissile, "Bala Mágica", "Magic Missile"),
            (LocKeys.WeaponOrbital, "Orbe Espectral", "Spectral Orb"),
            (LocKeys.WeaponLaser, "Haz Perforante", "Piercing Beam"),
            (LocKeys.WeaponChain, "Cadena Eléctrica", "Chain Lightning"),
            (LocKeys.WeaponDrone, "Dron Acompañante", "Companion Drone"),

            (LocKeys.HudLevel, "Nv. {0}", "Lv. {0}"),
            (LocKeys.HudKills, "Bajas: {0}", "Kills: {0}"),
            (LocKeys.HudStats, "Velocidad: {0}%   Daño: {1}%", "Speed: {0}%   Damage: {1}%"),
            (LocKeys.HudWeaponLevel, "{0} Nv.{1}", "{0} Lv.{1}"),

            (LocKeys.EndWin, "¡GANASTE!\n\nPresioná R para reintentar", "YOU WIN!\n\nPress R to retry"),
            (LocKeys.BsodMessage,
                "Tu PC tuvo un problema y tiene que reiniciarse.\n\nQué pena que hayas muerto... otra vez. ¿Reintentar?\n\nPresioná R para reintentar",
                "Your PC ran into a problem and needs to restart.\n\nSorry you died... again. Want to retry?\n\nPress R to retry"),

            (LocKeys.PauseTitle, "PAUSA", "PAUSED"),
            (LocKeys.PauseResume, "Reanudar", "Resume"),
            (LocKeys.PauseRestart, "Reiniciar", "Restart"),
            (LocKeys.PauseLanguage, "Idioma: Español", "Language: English"),

            (LocKeys.FlyTitle, "FLYWEIGHT (F3 oculta)", "FLYWEIGHT (F3 hides)"),
            (LocKeys.FlyEnemies, "Enemigos vivos: {0}  ->  datos compartidos (EnemyData): {1}",
                "Live enemies: {0}  ->  shared data (EnemyData): {1}"),
            (LocKeys.FlyProjectiles, "Proyectiles activos: {0}  ->  datos compartidos (WeaponData): {1}",
                "Active projectiles: {0}  ->  shared data (WeaponData): {1}"),
            (LocKeys.FlyNumbers, "Números de daño: {0}  ->  estilos compartidos: {1}",
                "Damage numbers: {0}  ->  shared styles: {1}"),
        };
    }
}
