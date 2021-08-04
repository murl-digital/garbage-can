using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Boosters;
using MediatR;

namespace GarbageCan.Application.Boosters.Commands
{
    public class AddSlotCommand : IRequest
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
    }

    public class AddSlotCommandHandler : IRequestHandler<AddSlotCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IBoosterService _boosterService;
        private readonly IDiscordGuildChannelService _discordChannelService;

        public AddSlotCommandHandler(IApplicationDbContext context, IBoosterService boosterService,
            IDiscordGuildChannelService discordChannelService)
        {
            _context = context;
            _boosterService = boosterService;
            _discordChannelService = discordChannelService;
        }

        public async Task<Unit> Handle(AddSlotCommand request, CancellationToken cancellationToken)
        {
            var slot = new AvailableSlot
            {
                GuildId = request.GuildId,
                ChannelId = request.ChannelId
            };
            _context.XPAvailableSlots.Add(slot);
            await _context.SaveChangesAsync(cancellationToken);
            _boosterService.AvailableSlots.TryAdd(request.GuildId, new List<AvailableSlot>());
            _boosterService.AvailableSlots[request.GuildId].Add(slot);

            _boosterService.QueuedBoosters.TryAdd(request.GuildId, new Queue<QueuedBooster>());
            _boosterService.ActiveBoosters.TryAdd(request.GuildId, new List<ActiveBooster>());

            await _discordChannelService.RenameChannel(request.GuildId, request.ChannelId, "-");

            return Unit.Value;
        }
    }
}
