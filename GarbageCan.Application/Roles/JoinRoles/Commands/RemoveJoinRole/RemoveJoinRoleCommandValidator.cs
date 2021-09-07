using FluentValidation;

namespace GarbageCan.Application.Roles.JoinRoles.Commands.RemoveJoinRole
{
    public class RemoveJoinRoleCommandValidator : AbstractValidator<RemoveJoinRoleCommand>
    {
        public RemoveJoinRoleCommandValidator()
        {
            RuleFor(v => v.Id).GreaterThan(0);
        }
    }
}
