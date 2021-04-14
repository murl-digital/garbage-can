using FluentValidation;

namespace GarbageCan.Application.Reactions.Commands.AssignRole
{
    public class AssignRoleCommandValidator : AbstractValidator<AssignRoleCommand>
    {
        public AssignRoleCommandValidator()
        {
            RuleFor(v => v.UserId).GreaterThan(0u);
            RuleFor(v => v.ChannelId).GreaterThan(0u);
            RuleFor(v => v.GuildId).GreaterThan(0u);
            RuleFor(v => v.MessageId).GreaterThan(0u);
            RuleFor(v => v.Emoji).NotNull();

            When(x => x.Emoji != null, () =>
            {
                RuleFor(x => x.Emoji.Name).NotNull().NotEmpty().When(x => x.Emoji.Id == 0);
            });
        }
    }
}