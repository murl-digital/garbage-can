using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Application.Roles.ReactionRoles.Commands.PrintReactionRoles
{
    public class PrintReactionRolesCommand : IRequest
    {
        public ulong GuildId { get; set; }
    }

    public class PrintReactionRolesCommandHandler : IRequestHandler<PrintReactionRolesCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IDiscordGuildService _guildService;
        private readonly IDiscordResponseService _responseService;

        public PrintReactionRolesCommandHandler(IDiscordResponseService responseService,
            IApplicationDbContext context,
            IDiscordGuildService guildService)
        {
            _responseService = responseService;
            _context = context;
            _guildService = guildService;
        }

        public async Task<Unit> Handle(PrintReactionRolesCommand request, CancellationToken cancellationToken)
        {
            var reactionRoles = await _context.ReactionRoles
                .Where(r => r.GuildId == request.GuildId)
                .ToListAsync(cancellationToken);
            if (!reactionRoles.Any())
            {
                await _responseService.RespondAsync("No reaction roles found!", formatAsBlock: true);
                return Unit.Value;
            }

            var roleNameDict = await _guildService.GetRoleNamesById(reactionRoles.Select(x => x.RoleId));
            var channelNameDict = await _guildService.GetChannelNamesById(reactionRoles.Select(x => x.ChannelId));

            var lines = reactionRoles
                .Select(x =>
                    $"{x.Id} :: msg {x.MessageId} in #{channelNameDict.GetValueOrDefault(x.ChannelId)} | {roleNameDict.GetValueOrDefault(x.RoleId)}")
                .ToList();
            await _responseService.RespondAsync(string.Join(Environment.NewLine, lines), formatAsBlock: true);

            return Unit.Value;
        }
    }
}
