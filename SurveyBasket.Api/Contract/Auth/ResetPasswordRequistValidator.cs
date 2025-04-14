using FluentValidation;
using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Contract.Auth;

public class ResetPasswordRequistValidator : AbstractValidator<ResetPasswordRequist>
{
    public ResetPasswordRequistValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Code)
            .NotEmpty();

        RuleFor(x => x.NewPaswood)
            .NotEmpty()
            .Matches(RegexPattern.Password)
            .WithMessage("Password should be at least 8 digits and should contain lowercase ,NonAlphanumeric , UpperCase");

    }
}
