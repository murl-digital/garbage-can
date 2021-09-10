using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using GarbageCan.Application.Fun.Commands.RandomStatus;
using GarbageCan.Domain.Enums;

namespace GarbageCan.Commands.Fun
{
    [RequireOwner]
    [Group("presence")]
    public class StatusCommand : MediatorCommandModule
    {
        [GroupCommand]
        public async Task Paulo(CommandContext ctx)
        {
            await Mediator.Send(new RandomizeStatusCommand(), ctx);
            await ctx.RespondAsync("shit");
        }

        [Command("add")]
        public async Task AddStatus(CommandContext ctx, Activity activity, [RemainingText] string name)
        {
            await Mediator.Send(new AddStatusCommand
            {
                Name = name,
                Activity = activity
            }, ctx);

            await Mediator.RespondAsync(ctx, "paulo");
        }
    }

    public class ActivityConverter : IArgumentConverter<Activity>
    {
        public Task<Optional<Activity>> ConvertAsync(string value, CommandContext ctx)
        {
            if (Enum.TryParse(value, out Activity result)) return Task.FromResult(Optional.FromValue(result));

            return value.ToLower() switch
            {
                "playing" => Task.FromResult(Optional.FromValue(Activity.Playing)),
                "streaming" => Task.FromResult(Optional.FromValue(Activity.Streaming)),
                "listening" => Task.FromResult(Optional.FromValue(Activity.ListeningTo)),
                "watching" => Task.FromResult(Optional.FromValue(Activity.Watching)),
                "competing" => Task.FromResult(Optional.FromValue(Activity.Competing)),
                _ => Task.FromResult(Optional.FromNoValue<Activity>())
            };
        }
    }
}
