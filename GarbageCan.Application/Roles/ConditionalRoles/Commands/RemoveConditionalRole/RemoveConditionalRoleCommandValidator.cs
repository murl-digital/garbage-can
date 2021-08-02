using FluentValidation;

namespace GarbageCan.Application.Roles.Commands.RemoveConditionalRole
{
    public class RemoveConditionalRoleCommandValidator : AbstractValidator<RemoveConditionalRoleCommand>
    {
        public RemoveConditionalRoleCommandValidator()
        {
            RuleFor(v => v.Id).GreaterThan(0);
        }
    }
}
