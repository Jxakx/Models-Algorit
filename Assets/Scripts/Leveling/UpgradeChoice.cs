using System;

namespace ArenaSurvivor.Leveling
{
    /// <summary>Una opción de mejora concreta: un título para mostrar y qué hacer al elegirla.</summary>
    public class UpgradeChoice
    {
        public string Title { get; }
        private readonly Action _apply;

        public UpgradeChoice(string title, Action apply)
        {
            Title = title;
            _apply = apply;
        }

        public void Apply() => _apply?.Invoke();
    }
}
