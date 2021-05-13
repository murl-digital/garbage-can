using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Infrastructure.Discord.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCan.Infrastructure.Discord
{
    public class DiscordGuildService : IDiscordGuildService
    {
        private readonly DiscordCommandContextService _contextService;
        private readonly ILogger<DiscordGuildService> _logger;

        public DiscordGuildService(DiscordCommandContextService contextService, ILogger<DiscordGuildService> logger)
        {
            _contextService = contextService;
            _logger = logger;
        }

        public Task<Dictionary<ulong, string>> GetChannelNamesById(IEnumerable<ulong> channelIds)
        {
            if (_contextService.CommandContext == null)
            {
                throw new CommandContextMissingException();
            }

            var roleDictionary = new Dictionary<ulong, string>();

            var guild = _contextService.CommandContext.Guild;
            try
            {
                foreach (var id in channelIds.Distinct())
                {
                    var channelName = guild.GetChannel(id)?.Name;
                    if (!string.IsNullOrWhiteSpace(channelName))
                    {
                        roleDictionary.Add(id, channelName);
                    }
                }

                return Task.FromResult(roleDictionary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Couldn't Role Id Names. Guild: {@guild} RoleIds: {ChannelIds} ", channelIds, new { guild.Id, guild.Name });
                throw;
            }
        }

        public async Task<string> GetMemberDisplayNameAsync(ulong userId)
        {
            if (_contextService.CommandContext == null)
            {
                throw new CommandContextMissingException();
            }

            var guild = _contextService.CommandContext.Guild;

            try
            {
                var member = await guild.GetMemberAsync(userId);
                return member?.DisplayName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Couldn't get guild for user: {userId} Guild: {@guild}", userId, new { guild.Id, guild.Name });
                throw;
            }
        }

        public Task<Dictionary<ulong, string>> GetRoleNamesById(IEnumerable<ulong> roleIds)
        {
            if (_contextService.CommandContext == null)
            {
                throw new CommandContextMissingException();
            }

            var roleDictionary = new Dictionary<ulong, string>();

            var guild = _contextService.CommandContext.Guild;
            try
            {
                foreach (var roleId in roleIds.Distinct())
                {
                    var roleName = guild.GetRole(roleId)?.Name;
                    if (!string.IsNullOrWhiteSpace(roleName))
                    {
                        roleDictionary.Add(roleId, roleName);
                    }
                }

                return Task.FromResult(roleDictionary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Couldn't Role Id Names. Guild: {@guild} RoleIds: {RoleIds} ", roleIds, new { guild.Id, guild.Name });
                throw;
            }
        }
    }
}