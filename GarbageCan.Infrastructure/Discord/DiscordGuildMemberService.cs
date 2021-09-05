using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Exceptions;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Common.Models;

namespace GarbageCan.Infrastructure.Discord
{
    public class DiscordGuildMemberService : IDiscordGuildMemberService
    {
        private readonly IDiscordConfiguration _configuration;
        private readonly DiscordClient _client;

        public DiscordGuildMemberService(IDiscordConfiguration configuration, DiscordClient client)
        {
            _configuration = configuration;
            _client = client;
        }

        public List<Member> GetGuildMembers()
        {
            return _client.Guilds[_configuration.GuildId].Members.Select(x => new Member
            {
                Id = x.Key,
                DisplayName = x.Value.DisplayName,
                IsBot = x.Value.IsBot
            }).ToList();
        }

        public Task<Member> GetMemberAsync(ulong id)
        {
            try
            {
                var dictionary = _client.Guilds[_configuration.GuildId].Members;
                if (!dictionary.ContainsKey(id))
                {
                    return Task.FromResult((Member)null);
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
                return Task.FromResult((Member)null);
            }
        }
    }
}
