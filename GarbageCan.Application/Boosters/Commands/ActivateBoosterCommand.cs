﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Boosters;
using MediatR;

namespace GarbageCan.Application.Boosters.Commands
{
    public class ActivateBoosterCommand : IRequest
    {
        public ulong GuildId { get; set; }
        public float Multiplier { get; set; }
        public TimeSpan Duration { get; set; }
        public AvailableSlot Slot { get; set; }
    }

    public class ActivateBoosterCommandHandler : IRequestHandler<ActivateBoosterCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IBoosterService _boosterService;
        private readonly IDateTime _dateTime;
        private readonly IDiscordGuildChannelService _discordChannelService;

        public ActivateBoosterCommandHandler(IApplicationDbContext context, IBoosterService boosterService,
            IDateTime dateTime, IDiscordGuildChannelService discordChannelService)
        {
            _context = context;
            _boosterService = boosterService;
            _dateTime = dateTime;
            _discordChannelService = discordChannelService;
        }

        public async Task<Unit> Handle(ActivateBoosterCommand request, CancellationToken cancellationToken)
        {
            if (!_boosterService.AvailableSlots.ContainsKey(request.GuildId))
                throw new InvalidOperationException("Specified guild has no available slots");

            if (!_boosterService.AvailableSlots[request.GuildId].Exists(s => s.Id == request.Slot.Id))
                throw new InvalidOperationException("Specified slot doesn't exist in guild");

            var booster = new ActiveBooster
            {
                GuildId = request.GuildId,
                ExpirationDate = _dateTime.Now.ToUniversalTime().Add(request.Duration),
                Multiplier = request.Multiplier,
                Slot = _context.XPAvailableSlots.First(s => s.Id == request.Slot.Id)
            };

            _boosterService.ActiveBoosters[request.GuildId].Add(booster);

            _context.XPActiveBoosters.Add(booster);
            await _context.SaveChangesAsync(cancellationToken);

            await _discordChannelService.RenameChannel(request.GuildId, request.Slot.ChannelId,
                $"{1 + request.Multiplier}");

            return Unit.Value;
        }
    }
}
