using FluentValidation;

namespace SurveyBasket.Api.Contract.Registeration;

public class ResendConfirmationEmailRequistValidators : AbstractValidator<ResendConfirmationEmailRequist>
{
    public ResendConfirmationEmailRequistValidators()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
