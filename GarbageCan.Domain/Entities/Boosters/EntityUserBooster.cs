﻿using System.ComponentModel.DataAnnotations;

namespace GarbageCan.Domain.Entities.Boosters
{
    public class EntityUserBooster
    {
        [Key] public int id { get; set; }
        public ulong userId { get; set; }
        public float multiplier { get; set; }
        public long durationInSeconds { get; set; }
    }
}