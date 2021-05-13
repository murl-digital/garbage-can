using GarbageCan.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.Application.Roles.Commands.PrintJoinRoles
{
    public class PrintJoinRolesCommand : IRequest
    {
    }

    public class PrintJoinRolesCommandHandler : IRequestHandler<PrintJoinRolesCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IDiscordGuildService _guildService;
        private readonly IDiscordResponseService _responseService;

        public PrintJoinRolesCommandHandler(IDiscordResponseService responseService,
            IApplicationDbContext context,
            IDiscordGuildService guildService)
        {
            _responseService = responseService;
            _context = context;
            _guildService = guildService;
        }

        public async Task<Unit> Handle(PrintJoinRolesCommand request, CancellationToken cancellationToken)
        {
            var joinRoles = await _context.JoinRoles.ToListAsync(cancellationToken);
            if (!joinRoles.Any())
            {
                await _responseService.RespondAsync("No join roles found!", formatAsBlock: true);
                return Unit.Value;
            }

            var roleDictionary = await _guildService.GetRoleNamesById(joinRoles.Select(x => x.roleId));

            var lines = joinRoles
                .Select(x => $"{x.id} :: {roleDictionary.GetValueOrDefault(x.roleId)}")
                .ToList();
            await _responseService.RespondAsync(string.Join(Environment.NewLine, lines), formatAsBlock: true);

            return Unit.Value;
        }
    }
}