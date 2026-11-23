using System;

namespace ArenaSurvivor.Leveling
{
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
