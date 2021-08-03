using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Boosters;
using GarbageCan.Domain.Enums;
using GarbageCan.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace GarbageCan.Infrastructure.Services
{
    public class BoosterService : IBoosterService
    {
        public ReadOnlyDictionary<ulong, ReadOnlyCollection<ActiveBooster>> ActiveBoosters => new(
            _activeBoosters.ToDictionary(k => k.Key, v => v.Value.AsReadOnly())
        );

        public ReadOnlyDictionary<ulong, ReadOnlyCollection<QueuedBooster>> QueuedBoosters => new(
            _queuedBoosters.ToDictionary(k => k.Key, v => v.Value.ToList().AsReadOnly())
        );

        public ReadOnlyDictionary<ulong, ReadOnlyCollection<AvailableSlot>> AvailableSlots => new(
            _availableSlots.ToDictionary(k => k.Key, v => v.Value.AsReadOnly())
        );

        private Dictionary<ulong, List<ActiveBooster>> _activeBoosters;
        private Dictionary<ulong, Queue<QueuedBooster>> _queuedBoosters;
        private Dictionary<ulong, List<AvailableSlot>> _availableSlots;

        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IDateTime _dateTime;

        public BoosterService(IDbContextFactory<ApplicationDbContext> contextFactory, IDateTime dateTime)
        {
            _contextFactory = contextFactory;
            _dateTime = dateTime;
        }

        public async void Init()
        {
            await using var context = _contextFactory.CreateDbContext();
            _availableSlots = await context.XPAvailableSlots
                .AsNoTracking()
                .GroupBy(s => s.GuildId)
                .ToDictionaryAsync(s => s.Key, s => s.ToList());
            _queuedBoosters = await context.XPQueuedBoosters
                .AsNoTracking()
                .GroupBy(b => b.GuildId)
                .ToDictionaryAsync(k => k.Key, v => new Queue<QueuedBooster>(v.OrderBy(b => b.Position).ToList()));
            _activeBoosters = await context.XPActiveBoosters
                .AsNoTracking()
                .GroupBy(b => b.GuildId)
                .ToDictionaryAsync(k => k.Key, v => v.ToList());
        }

        public float GetMultiplier(ulong guildId)
        {
            return 1 + _activeBoosters[guildId].Sum(b => b.Multiplier);
        }

        public async Task<BoosterResult> AddBooster(ulong guildId, float multiplier, TimeSpan duration, bool queue)
        {
            if (!_availableSlots.ContainsKey(guildId))
                throw new InvalidOperationException("Specified guild has no available slots");

            if (_activeBoosters[guildId].Count >= _availableSlots[guildId].Count)
            {
                if (!queue) return BoosterResult.SlotsFull;

                _queuedBoosters[guildId].Enqueue(new QueuedBooster
                {
                    Multiplier = multiplier,
                    DurationInSeconds = (long)duration.TotalSeconds
                });
                await SaveQueue(guildId);
                return BoosterResult.Queued;
            }

            var usedSlots = _activeBoosters[guildId]
                .Select(b => b.Slot)
                .ToList();

            var slot = _availableSlots[guildId]
                .First(s => !usedSlots.Contains(s));

            ActivateBooster(guildId, multiplier, duration, slot);
            return BoosterResult.Active;
        }

        public async Task AddSlot(ulong guildId, ulong channelId)
        {
            await using var context = _contextFactory.CreateDbContext();
            var slot = new AvailableSlot
            {
                GuildId = guildId,
                ChannelId = channelId
            };
            context.XPAvailableSlots.Add(slot);
            await context.SaveChangesAsync();
            _availableSlots.TryAdd(guildId, new List<AvailableSlot>());
            _availableSlots[guildId].Add(slot);
        }

        public async Task RemoveSlot(ulong guildId, int id)
        {
            if (!_availableSlots.ContainsKey(guildId))
                throw new InvalidOperationException("Invalid guild ID");
            if (_availableSlots[guildId].All(s => s.Id != id))
                throw new ArgumentException("Invalid slot ID");

            await using var context = _contextFactory.CreateDbContext();
            context.XPAvailableSlots.Remove(await context.XPAvailableSlots.FirstAsync(s => s.Id == id));
            await context.SaveChangesAsync();
            _availableSlots[guildId].RemoveAll(s => s.Id == id);
        }

        public async Task SaveQueue(ulong guildId)
        {
            await using var context = _contextFactory.CreateDbContext();
            await context.XPQueuedBoosters
                .Where(b => b.GuildId == guildId)
                .DeleteAsync();

            uint position = 0;
            foreach (var booster in _queuedBoosters[guildId])
            {
                context.XPQueuedBoosters.Add(new QueuedBooster
                {
                    GuildId = guildId,
                    Multiplier = booster.Multiplier,
                    DurationInSeconds = booster.DurationInSeconds,
                    Position = position
                });
                position++;
            }

            await context.SaveChangesAsync();
        }

        private async void ActivateBooster(ulong guildId, float multiplier, TimeSpan duration, AvailableSlot slot)
        {
            if (!_availableSlots.ContainsKey(guildId))
                throw new InvalidOperationException("Specified guild has no available slots");

            if (_availableSlots[guildId].All(s => s.Id == slot.Id))
                throw new InvalidOperationException("Specified slot doesn't exist in guild");

            var booster = new ActiveBooster
            {
                GuildId = guildId,
                ExpirationDate = _dateTime.Now.ToUniversalTime().Add(duration),
                Multiplier = multiplier,
                Slot = slot
            };

            _activeBoosters[guildId].Add(booster);

            await using var context = _contextFactory.CreateDbContext();
            context.XPActiveBoosters.Add(booster);
            await context.SaveChangesAsync();
        }
    }
}
