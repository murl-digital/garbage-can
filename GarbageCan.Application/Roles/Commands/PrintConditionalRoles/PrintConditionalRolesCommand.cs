using GarbageCan.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.Application.Roles.Commands.PrintConditionalRoles
{
    public class PrintConditionalRolesCommand : IRequest
    {
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
            var conditionalRoles = await _context.ConditionalRoles.ToListAsync(cancellationToken);
            if (!conditionalRoles.Any())
            {
                await _responseService.RespondAsync("No conditional roles found!", formatAsBlock: true);
                return Unit.Value;
            }

            var roleIds = conditionalRoles.Select(x => x.resultRoleId).Concat(conditionalRoles.Select(x => x.requiredRoleId));
            var roleDictionary = await _guildService.GetRoleNamesById(roleIds);

            var lines = conditionalRoles
                .Select(x => $"{x.id} :: {roleDictionary.GetValueOrDefault(x.requiredRoleId)} | {roleDictionary.GetValueOrDefault(x.resultRoleId)}")
                .ToList();
            await _responseService.RespondAsync(string.Join(Environment.NewLine, lines), formatAsBlock: true);

            return Unit.Value;
        }
    }
}