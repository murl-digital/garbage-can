using GarbageCan.Domain.Enums;

namespace GarbageCan.Application.Common.Interfaces
{
    public interface IDiscordConnectionService
    {
        public DiscordConnectionStatus Status { get; }
    }
}