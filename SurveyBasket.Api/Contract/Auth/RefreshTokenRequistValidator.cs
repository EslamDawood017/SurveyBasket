using FluentValidation;

namespace SurveyBasket.Api.Contract.Auth;

public class RefreshTokenRequistValidator : AbstractValidator<RefreshTokenRequist>
{
    public RefreshTokenRequistValidator()
    {
        RuleFor(x => x.token).NotEmpty();

        RuleFor(x => x.refreshToken).NotEmpty();

    }
}