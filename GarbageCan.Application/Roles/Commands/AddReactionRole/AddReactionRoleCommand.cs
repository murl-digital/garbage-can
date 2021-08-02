﻿using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities;
using GarbageCan.Domain.Entities.Roles;
using MediatR;

namespace GarbageCan.Application.Roles.Commands.AddReactionRole
{
    public class AddReactionRoleCommand : IRequest
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }
        public Emoji Emoji { get; set; }
        public ulong RoleId { get; set; }
    }

    public class AddReactionRoleCommandHandler : IRequestHandler<AddReactionRoleCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IDiscordMessageService _messageService;
        private readonly IDiscordResponseService _responseService;

        public AddReactionRoleCommandHandler(IApplicationDbContext context, IDiscordResponseService responseService,
            IDiscordMessageService messageService)
        {
            _context = context;
            _responseService = responseService;
            _messageService = messageService;
        }

        public async Task<Unit> Handle(AddReactionRoleCommand request, CancellationToken cancellationToken)
        {
            await _context.ReactionRoles.AddAsync(new ReactionRole
            {
                RoleId = request.RoleId,
                GuildId = request.GuildId,
                ChannelId = request.ChannelId,
                MessageId = request.MessageId,
                EmoteId = EmoteId(request.Emoji)
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            await _messageService.CreateReactionAsync(request.ChannelId, request.MessageId, request.Emoji);
            await _responseService.RespondAsync("Role added successfully", true);
            return Unit.Value;
        }

        private static string EmoteId(Emoji emote)
        {
            return emote.Id == 0 ? emote.Name : emote.Id.ToString();
        }
    }
}
