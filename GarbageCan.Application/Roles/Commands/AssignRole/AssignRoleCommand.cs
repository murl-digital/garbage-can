using System;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
            await _context.reactionRoles.ForEachAsync(async r =>
            {
                try
                {
                    // before you say "BUT JOE YOU CAN USE .WHERE() JOE" hear me out
                    // i tried that. i really did. but for some reason it didnt work.
                    // it would just say "hey all these rows satisfy the predicate!" ...even though they don't
                    // conclusion: linq is a lie thank you for coming to my ted talk
                    if (r.channelId != request.ChannelId || r.messageId != request.MessageId ||
                        r.emoteId != EmoteId(request.Emoji)) return;

                    await _roleService.GrantRoleAsync(request.GuildId, r.roleId, request.UserId, "reaction role");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Couldn't assign reaction role");
                }
            }, cancellationToken);

            return true;
        }

        private static string EmoteId(Emoji emote)
        {
            return emote.Id == 0 ? emote.Name : emote.Id.ToString();
        }
    }
}