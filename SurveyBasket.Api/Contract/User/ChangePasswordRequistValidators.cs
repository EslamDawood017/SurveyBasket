using FluentValidation;
using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Contract.User;

public class ChangePasswordRequistValidators : AbstractValidator<ChangePasswordRequist>
{
    public ChangePasswordRequistValidators()
    {
        RuleFor(x => x.CurrentPassword)
           .NotEmpty();

        RuleFor(x => x.NewPassword)
           .NotEmpty()
           .Matches(RegexPattern.Password)
           .WithMessage("Password should be at least 8 digits and should contain lowercase ,NonAlphanumeric , UpperCase")
           .NotEqual(x => x.CurrentPassword)
           .WithMessage("New Password can't be the same as the current password");

    }
}
