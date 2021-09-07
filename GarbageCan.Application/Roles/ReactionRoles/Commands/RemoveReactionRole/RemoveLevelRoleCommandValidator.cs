using FluentValidation;

namespace GarbageCan.Application.Roles.ReactionRoles.Commands.RemoveReactionRole
{
    public class RemoveReactionRoleCommandValidator : AbstractValidator<RemoveReactionRoleCommand>
    {
        public RemoveReactionRoleCommandValidator()
        {
            RuleFor(v => v.Id).GreaterThan(0);
        }
    }
}
