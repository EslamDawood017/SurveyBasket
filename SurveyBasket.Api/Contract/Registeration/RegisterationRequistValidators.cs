using FluentValidation;
using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Contract.Registeration;

public class RegisterationRequistValidators : AbstractValidator<RegisterationRequist>
{
    public RegisterationRequistValidators()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .Matches(RegexPattern.Password)
            .WithMessage("Password should be at least 8 digits and should contain lowercase ,NonAlphanumeric , UpperCase");

        RuleFor(x => x.FirstName)
           .NotEmpty()
           .Length(3,100);

        RuleFor(x => x.LastName)
           .NotEmpty()
           .Length(3, 100); 
    }
}
