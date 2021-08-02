using FluentValidation;

namespace GarbageCan.Application.Roles.Commands.AddReactionRole
{
    public class AddReactionRoleCommandValidator : AbstractValidator<AddReactionRoleCommand>
    {
        public AddReactionRoleCommandValidator()
        {
            RuleFor(v => v.RoleId).GreaterThan(0u);
            RuleFor(v => v.ChannelId).GreaterThan(0u);
            RuleFor(v => v.MessageId).GreaterThan(0u);
            RuleFor(v => v.Emoji).NotNull();
        }
    }
}
