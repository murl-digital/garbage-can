﻿namespace GarbageCan.Data.Models.Boosters
{
    public class UserBooster
    {
        public string id { get; set; }
        public string userId { get; set; }
        public float multiplier { get; set; }
        public long durationInSeconds { get; set; }
    }
}