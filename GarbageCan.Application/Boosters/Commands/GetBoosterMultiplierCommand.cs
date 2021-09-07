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
            return _boosterService.ActiveBoosters.TryGetValue(request.GuildId, out var boosters)
                ? Normalize(boosters.Sum(b => b.Multiplier))
                : 1;
        }

        private static float Normalize(float num)
        {
            return num == 0 ? 1 : num;
        }
    }
}
