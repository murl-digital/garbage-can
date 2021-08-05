using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Application.Roles.JoinRoles.Commands.PrintJoinRoles
{
    public class PrintJoinRolesCommand : IRequest
    {
        public ulong GuildId { get; set; }
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
            var joinRoles = await _context.JoinRoles
                .Where(r => r.GuildId == request.GuildId)
                .ToListAsync(cancellationToken);
            if (!joinRoles.Any())
            {
                await _responseService.RespondAsync("No join roles found!", formatAsBlock: true);
                return Unit.Value;
            }

            var roleDictionary = await _guildService.GetRoleNamesById(joinRoles.Select(x => x.RoleId));

            var lines = joinRoles
                .Select(x => $"{x.Id} :: {roleDictionary.GetValueOrDefault(x.RoleId)}")
                .ToList();
            await _responseService.RespondAsync(string.Join(Environment.NewLine, lines), formatAsBlock: true);

            return Unit.Value;
        }
    }
}
