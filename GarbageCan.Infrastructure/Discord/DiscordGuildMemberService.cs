using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Common.Models;

namespace GarbageCan.Infrastructure.Discord
{
    public class DiscordGuildMemberService : IDiscordGuildMemberService
    {
        private readonly DiscordClient _client;

        public DiscordGuildMemberService(DiscordClient client)
        {
            _client = client;
        }

        public List<Member> GetGuildMembers(ulong? guildId)
        {
            var guild = GetGuild(guildId);
            return guild?.Members.Select(x => new Member
            {
                Id = x.Key,
                DisplayName = x.Value.DisplayName,
                IsBot = x.Value.IsBot,
                IsPending = x.Value.IsPending ?? true
            }).ToList();
        }

        public Task<Member> GetMemberAsync(ulong? guildId, ulong id)
        {
            try
            {
                var guild = GetGuild(guildId);
                var dictionary = guild.Members;
                if (!dictionary.ContainsKey(id))
                {
                    return Task.FromResult((Member) null);
                }

                var member = dictionary[id];
                return Task.FromResult(new Member
                {
                    Id = member.Id,
                    DisplayName = member.DisplayName,
                    IsBot = member.IsBot
                });
            }
            catch (NotFoundException)
            {
                return Task.FromResult((Member) null);
            }
        }

        private DiscordGuild GetGuild(ulong? guildId)
        {
            var guild = guildId.HasValue ? _client.Guilds[guildId.Value] : _client.Guilds.Values.FirstOrDefault();
            return guild;
        }
    }
}
