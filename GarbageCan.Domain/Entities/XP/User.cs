using System;

namespace GarbageCan.Domain.Entities.XP
{
    public class User
    {
        private double _xp;

        public ulong Id { get; set; }

        public int Lvl { get; set; }

        public double XP
        {
            get => _xp;
            set => _xp = Math.Round(value, 1);
        }
    }
}