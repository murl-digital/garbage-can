using FluentValidation;

namespace GarbageCan.Application.Roles.LevelRoles.Commands.AddLevelRole
{
    public class AddLevelRoleCommandValidator : AbstractValidator<AddLevelRoleCommand>
    {
        public AddLevelRoleCommandValidator()
        {
            RuleFor(v => v.RoleId).GreaterThan(0u);
            RuleFor(x => x.Level).GreaterThan(0);
        }
    }
}
