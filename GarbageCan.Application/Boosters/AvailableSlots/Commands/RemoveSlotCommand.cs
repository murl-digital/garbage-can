using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Application.Boosters.AvailableSlots.Commands
{
    public class RemoveSlotCommand : IRequest
    {
        public ulong GuildId { get; set; }
        public int Id { get; set; }
    }

    public class RemoveSlotCommandHandler : IRequestHandler<RemoveSlotCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IBoosterService _boosterService;

        public RemoveSlotCommandHandler(IApplicationDbContext context, IBoosterService boosterService)
        {
            _context = context;
            _boosterService = boosterService;
        }

        public async Task<Unit> Handle(RemoveSlotCommand request, CancellationToken cancellationToken)
        {
            if (!_boosterService.AvailableSlots.ContainsKey(request.GuildId))
                throw new InvalidOperationException("Invalid guild ID");
            if (_boosterService.AvailableSlots[request.GuildId].All(s => s.Id != request.Id))
                throw new ArgumentException("Invalid slot ID");

            _context.XPAvailableSlots.Remove(
                await _context.XPAvailableSlots.FirstAsync(s => s.Id == request.Id, cancellationToken));
            await _context.SaveChangesAsync(cancellationToken);
            _boosterService.AvailableSlots[request.GuildId].RemoveAll(s => s.Id == request.Id);

            return Unit.Value;
        }
    }
}
