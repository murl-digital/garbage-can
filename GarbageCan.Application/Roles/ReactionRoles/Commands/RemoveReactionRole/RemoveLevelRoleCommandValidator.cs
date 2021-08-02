using FluentValidation;

namespace GarbageCan.Application.Roles.Commands.RemoveReactionRole
{
    public class RemoveReactionRoleCommandValidator : AbstractValidator<RemoveReactionRoleCommand>
    {
        public RemoveReactionRoleCommandValidator()
        {
            RuleFor(v => v.Id).GreaterThan(0);
        }
    }
}
