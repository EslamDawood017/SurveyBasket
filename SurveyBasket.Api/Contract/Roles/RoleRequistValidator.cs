using FluentValidation;

namespace SurveyBasket.Api.Contract.Roles;

public class RoleRequistValidator : AbstractValidator<RoleRequist>
{
    public RoleRequistValidator()
    {
        RuleFor(x => x.Name)
             .NotEmpty()
             .Length(3, 256);

        RuleFor(x => x.Permission)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Permission)
            .Must(x => x.Distinct().Count() == x.Count())
            .WithMessage("You can't add duplicated permissions")
            .When(x => x.Permission != null);
    }
}
