using GarbageCan.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.Application.Roles.Commands.PrintReactionRoles
{
    public class PrintReactionRolesCommand : IRequest
    {
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
            var reactionRoles = await _context.ReactionRoles.ToListAsync(cancellationToken);
            if (!reactionRoles.Any())
            {
                await _responseService.RespondAsync("No reaction roles found!", formatAsBlock: true);
                return Unit.Value;
            }

            var roleNameDict = await _guildService.GetRoleNamesById(reactionRoles.Select(x => x.roleId));
            var channelNameDict = await _guildService.GetChannelNamesById(reactionRoles.Select(x => x.channelId));

            var lines = reactionRoles
                .Select(x => $"{x.id} :: msg {x.messageId} in #{channelNameDict.GetValueOrDefault(x.channelId)} | {roleNameDict.GetValueOrDefault(x.roleId)}")
                .ToList();
            await _responseService.RespondAsync(string.Join(Environment.NewLine, lines), formatAsBlock: true);

            return Unit.Value;
        }
    }
}