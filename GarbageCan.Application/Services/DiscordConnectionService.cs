using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Enums;

namespace GarbageCan.Application.Services
{
    public class DiscordConnectionService : IDiscordConnectionService
    {
        public DiscordConnectionStatus Status { get; set; }
    }
}