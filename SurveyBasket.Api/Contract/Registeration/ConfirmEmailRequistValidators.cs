using FluentValidation;

namespace SurveyBasket.Api.Contract.Registeration;

public class ConfirmEmailRequistValidators : AbstractValidator<ConfirmEmailRequist>
{
    public ConfirmEmailRequistValidators()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Code).NotEmpty();
    }
}
