using FluentValidation;

namespace GarbageCan.Application.Roles.Commands.AddConditionalRole
{
    public class AddConditionalRoleCommandValidator : AbstractValidator<AddConditionalRoleCommand>
    {
        public AddConditionalRoleCommandValidator()
        {
            RuleFor(v => v.RequiredRoleId).GreaterThan(0u);
            RuleFor(v => v.ResultRoleId).GreaterThan(0u);
        }
    }
}