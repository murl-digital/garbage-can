﻿namespace GarbageCan.Application.Common.Interfaces
{
    public interface IRngService
    {
        public double NormalSample { get; }
        float FloatFromRange(float lower, float higher);
    }
}
