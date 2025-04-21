using FluentValidation;

namespace SurveyBasket.Api.Contract.User;

public class UpdateUserProfileRequistValidators : AbstractValidator<UpdateUserProfileRequist>
{
    public UpdateUserProfileRequistValidators()
    {
        RuleFor(x => x.FirstName)
           .NotEmpty()
           .Length(3, 100);

        RuleFor(x => x.LastName)
           .NotEmpty()
           .Length(3, 100);
    }
}
