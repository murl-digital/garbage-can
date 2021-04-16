using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.Application.Roles.Commands.AssignRole
{
    public class AssignRoleCommand : IRequest<bool>
    {
        public ulong ChannelId { get; set; }
        public Emoji Emoji { get; set; }
        public ulong GuildId { get; set; }
        public ulong MessageId { get; set; }
        public ulong UserId { get; set; }
    }

    public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<AssignRoleCommandHandler> _logger;
        private readonly IDiscordGuildRoleService _roleService;

        public AssignRoleCommandHandler(IApplicationDbContext context,
            IDiscordGuildRoleService roleService,
            ILogger<AssignRoleCommandHandler> logger)
        {
            _context = context;
            _roleService = roleService;
            _logger = logger;
        }

        public async Task<bool> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
        {
            var roles = await _context.reactionRoles
                .Where(x => x.channelId == request.ChannelId && x.messageId == request.MessageId)
                .ToListAsync(cancellationToken);

            roles = roles.Where(r => r.emoteId == EmoteId(request.Emoji)).ToList();

            foreach (var reactionRole in roles)
            {
                try
                {
                    await _roleService.GrantRoleAsync(request.GuildId, reactionRole.roleId, request.UserId, "reaction role");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Couldn't assign reaction role");
                }
            }

            return true;
        }

        private static string EmoteId(Emoji emote)
        {
            return emote.Id == 0 ? emote.Name : emote.Id.ToString();
        }
    }
}