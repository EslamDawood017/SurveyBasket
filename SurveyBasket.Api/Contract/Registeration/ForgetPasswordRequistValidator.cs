using FluentValidation;

namespace SurveyBasket.Api.Contract.Registeration;

public class ForgetPasswordRequistValidator : AbstractValidator<ForgetPasswordRequist>
{
    public ForgetPasswordRequistValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
