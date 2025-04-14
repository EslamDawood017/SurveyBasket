using FluentValidation;

namespace SurveyBasket.Api.Contract.Auth;

public class ResendConfirmationEmailRequistValidators : AbstractValidator<ResendConfirmationEmailRequist>
{
    public ResendConfirmationEmailRequistValidators()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
