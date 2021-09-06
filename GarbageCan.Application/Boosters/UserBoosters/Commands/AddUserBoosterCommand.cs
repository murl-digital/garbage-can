using System;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Boosters;
using MediatR;

namespace GarbageCan.Application.Boosters.UserBoosters.Commands
{
    public class AddUserBoosterCommand : IRequest
    {
        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }
        public float Multiplier { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public class AddUserBoosterCommandHandler : IRequestHandler<AddUserBoosterCommand>
    {
        private readonly IApplicationDbContext _context;

        public AddUserBoosterCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(AddUserBoosterCommand request, CancellationToken cancellationToken)
        {
            _context.XPUserBoosters
                .Add(new UserBooster
                {
                    GuildId = request.GuildId,
                    UserId = request.UserId,
                    Multiplier = request.Multiplier,
                    DurationInSeconds = (ulong)request.Duration.TotalSeconds
                });

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
