using System;
using GarbageCan.Application.Common.Interfaces;
using MathNet.Numerics.Distributions;

namespace GarbageCan.Infrastructure.Services
{
    public class RngService : IRngService
    {
        private readonly Normal _normal = new(3, 7);
        private readonly Random _random = new();

        public double NormalSample => _normal.Sample();

        public float FloatFromRange(float lower, float higher)
        {
            return (float)(_random.NextDouble() * (higher - lower + 1) + lower);
        }
    }
}
