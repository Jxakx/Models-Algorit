namespace ArenaSurvivor.Core.Audio
{
    /// <summary>
    /// Nombre de cada sonido del juego. Es la "clave" de la Look-Up Table de SoundBank:
    /// quien quiere que suene algo pide un SoundId, nunca un AudioClip ni una ruta de
    /// archivo — qué clip hay detrás de cada nombre se decide en el Inspector.
    ///
    /// Vive en Core (y no en Audio) porque lo comparten los datos de armas (WeaponData
    /// dice con qué sonido dispara) y el reproductor, sin que uno dependa del otro.
    ///
    /// Los valores se agregan al FINAL: Unity serializa el enum como número, así que
    /// reordenar los existentes cambiaría qué sonido tiene asignado cada entrada ya guardada.
    /// </summary>
    public enum SoundId
    {
        Music,
        Shoot,
        Laser,
        ChainLightning,
        Impact,
        EnemyDeath,
        PlayerHurt,
        XpPickup,
        LevelUp,
        UpgradeConfirm,
        ButtonClick,
        GameOver,
        Victory,
        PauseOpen,
        PauseClose
    }
}
