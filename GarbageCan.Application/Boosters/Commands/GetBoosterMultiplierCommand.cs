using System.Linq;
using GarbageCan.Application.Common.Interfaces;
using MediatR;

namespace GarbageCan.Application.Boosters.Commands
{
    public class GetBoosterMultiplierCommand : IRequest<float>
    {
        public ulong GuildId { get; set; }
    }

    public class GetBoosterMultiplierCommandHandler : RequestHandler<GetBoosterMultiplierCommand, float>
    {
        private readonly IBoosterService _boosterService;

        public GetBoosterMultiplierCommandHandler(IBoosterService boosterService)
        {
            _boosterService = boosterService;
        }

        protected override float Handle(GetBoosterMultiplierCommand request)
        {
            return 1 + (_boosterService.ActiveBoosters.TryGetValue(request.GuildId, out var boosters)
                ? boosters?.Sum(b => b.Multiplier) ?? 0
                : 0);
        }
    }
}
