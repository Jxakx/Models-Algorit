namespace ArenaSurvivor.Core.Events
{
    /// <summary>
    /// Marca a una clase/struct como un evento válido para publicar a través del EventManager.
    /// No tiene miembros a propósito: solo sirve para que el compilador nos obligue a pasar
    /// por acá cualquier cosa que se publique, en vez de aceptar cualquier objeto suelto.
    /// </summary>
    public interface IGameEvent
    {
    }
}
