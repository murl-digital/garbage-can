using GarbageCan.Application.Common.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.Application.Fun.Commands.OobifyText
{
    public class OobifyTextCommand : IRequest<bool>
    {
        public string Text { get; set; }
    }

    public class OobifyTextCommandHandler : IRequestHandler<OobifyTextCommand, bool>
    {
        private readonly IDiscordResponseService _responseService;

        public OobifyTextCommandHandler(IDiscordResponseService responseService)
        {
            _responseService = responseService;
        }

        public async Task<bool> Handle(OobifyTextCommand request, CancellationToken cancellationToken)
        {
            var result = request.Text
                .Replace("a", "oob")
                .Replace("e", "oob")
                .Replace("i", "oob")
                .Replace("o", "oob")
                .Replace("u", "oob")
                .Replace("A", "Oob")
                .Replace("E", "Oob")
                .Replace("I", "Oob")
                .Replace("O", "Oob")
                .Replace("U", "Oob");

            await _responseService.RespondAsync(result);
            return true;
        }
    }
}