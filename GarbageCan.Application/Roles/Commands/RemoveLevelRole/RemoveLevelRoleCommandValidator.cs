﻿using FluentValidation;

namespace GarbageCan.Application.Roles.Commands.RemoveLevelRole
{
    public class RemoveLevelRoleCommandValidator : AbstractValidator<RemoveLevelRoleCommand>
    {
        public RemoveLevelRoleCommandValidator()
        {
            RuleFor(v => v.Id).GreaterThan(0);
        }
    }
}