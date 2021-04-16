using FluentValidation;

namespace GarbageCan.Application.Roles.Commands.AlterRole
{
    public class AlterRoleCommandValidator : AbstractValidator<AlterRoleCommand>
    {
        public AlterRoleCommandValidator()
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