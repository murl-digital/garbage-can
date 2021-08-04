using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Application.Boosters.Commands
{
    public class RemoveSlotCommand : IRequest
    {
        public ulong guildId { get; set; }
        public int id { get; set; }
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
            if (!_boosterService.AvailableSlots.ContainsKey(request.guildId))
                throw new InvalidOperationException("Invalid guild ID");
            if (_boosterService.AvailableSlots[request.guildId].All(s => s.Id != request.id))
                throw new ArgumentException("Invalid slot ID");

            _context.XPAvailableSlots.Remove(
                await _context.XPAvailableSlots.FirstAsync(s => s.Id == request.id, cancellationToken));
            await _context.SaveChangesAsync(cancellationToken);
            _boosterService.AvailableSlots[request.guildId].RemoveAll(s => s.Id == request.id);

            return Unit.Value;
        }
    }
}
