using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using MediatR;

namespace GarbageCan.Application.Boosters.Commands
{
    public class GetBoosterMultiplierCommand : IRequest<float>
    {
        public ulong GuildId { get; set; }
    }

    public class GetBoosterMultiplierCommandHandler : IRequestHandler<GetBoosterMultiplierCommand, float>
    {
        private readonly IBoosterService _boosterService;

        public GetBoosterMultiplierCommandHandler(IBoosterService boosterService)
        {
            _boosterService = boosterService;
        }

        public Task<float> Handle(GetBoosterMultiplierCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(1 + _boosterService.ActiveBoosters[request.GuildId]?.Sum(b => b.Multiplier) ?? 0);
        }
    }
}
