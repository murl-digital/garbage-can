using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Application.Roles.Commands.PrintConditionalRoles
{
    public class PrintConditionalRolesCommand : IRequest
    {
        public ulong GuildId { get; set; }
    }

    public class PrintConditionalRolesCommandHandler : IRequestHandler<PrintConditionalRolesCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IDiscordGuildService _guildService;
        private readonly IDiscordResponseService _responseService;

        public PrintConditionalRolesCommandHandler(IDiscordResponseService responseService,
            IApplicationDbContext context,
            IDiscordGuildService guildService)
        {
            _responseService = responseService;
            _context = context;
            _guildService = guildService;
        }

        public async Task<Unit> Handle(PrintConditionalRolesCommand request, CancellationToken cancellationToken)
        {
            var conditionalRoles = await _context.ConditionalRoles
                .Where(r => r.GuildId == request.GuildId)
                .ToListAsync(cancellationToken);
            if (!conditionalRoles.Any())
            {
                await _responseService.RespondAsync("No conditional roles found!", formatAsBlock: true);
                return Unit.Value;
            }

            var roleIds = conditionalRoles.Select(x => x.ResultRoleId)
                .Concat(conditionalRoles.Select(x => x.RequiredRoleId));
            var roleDictionary = await _guildService.GetRoleNamesById(roleIds);

            var lines = conditionalRoles
                .Select(x =>
                    $"{x.Id} :: {roleDictionary.GetValueOrDefault(x.RequiredRoleId)} | {roleDictionary.GetValueOrDefault(x.ResultRoleId)}")
                .ToList();
            await _responseService.RespondAsync(string.Join(Environment.NewLine, lines), formatAsBlock: true);

            return Unit.Value;
        }
    }
}
