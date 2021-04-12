using GarbageCan.Application.Common.Interfaces;

namespace GarbageCan.Infrastructure.Services
{
    public class DummyUserService : ICurrentUserService
    {
        public string UserId { get; } = "DUMMY";
    }
}