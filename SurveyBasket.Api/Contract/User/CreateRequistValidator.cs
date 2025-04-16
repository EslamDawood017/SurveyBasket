using FluentValidation;
using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Contract.User;

public class CreateRequistValidator : AbstractValidator<CreateUserRequist>
{
    public CreateRequistValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Length(3,100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .Matches(RegexPattern.Password)
            .WithMessage("Password should be at least 8 digits and should contain lowercase ,NonAlphanumeric , UpperCase");

        RuleFor(x => x.Roles)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.Roles)
            .Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("You can't Duplicate role for the same user")
            .When(x => x.Roles != null);


    }
}
