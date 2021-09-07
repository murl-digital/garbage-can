using FluentValidation;

namespace GarbageCan.Application.Roles.JoinRoles.Commands.AddJoinRole
{
    public class AddJoinRoleCommandValidator : AbstractValidator<AddJoinRoleCommand>
    {
        public AddJoinRoleCommandValidator()
        {
            RuleFor(v => v.RoleId).GreaterThan(0u);
        }
    }
}
