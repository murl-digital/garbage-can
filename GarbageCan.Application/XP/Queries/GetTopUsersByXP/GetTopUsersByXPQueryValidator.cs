using FluentValidation;

namespace GarbageCan.Application.XP.Queries.GetTopUsersByXP
{
    public class GetTopUsersByXPQueryValidator : AbstractValidator<GetTopUsersByXPQuery>
    {
        public GetTopUsersByXPQueryValidator()
        {
            RuleFor(v => v.CurrentUserId).GreaterThan(0u).WithMessage("The current user is required");
            RuleFor(x => x.Count).GreaterThan(0);
        }
    }
}