using FluentValidation;

namespace GarbageCan.Application.Roles.Commands.RemoveJoinRole
{
    public class RemoveJoinRoleCommandValidator : AbstractValidator<RemoveJoinRoleCommand>
    {
        public RemoveJoinRoleCommandValidator()
        {
            RuleFor(v => v.Id).GreaterThan(0);
        }
    }
}