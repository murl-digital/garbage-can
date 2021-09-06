using GarbageCan.Application.Common.Interfaces;
using MathNet.Numerics.Distributions;

namespace GarbageCan.Infrastructure.Services
{
    public class RngService : IRngService
    {
        private readonly Normal _normal = new(3, 7);

        public double NormalSample => _normal.Sample();
    }
}
