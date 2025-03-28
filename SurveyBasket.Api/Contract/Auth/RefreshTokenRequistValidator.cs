using FluentValidation;
using Microsoft.AspNetCore.Identity.Data;

namespace SurveyBasket.Api.Contract.Auth;

public class RefreshTokenRequistValidator : AbstractValidator<RefreshTokenRequist>
{
    public RefreshTokenRequistValidator()
    {
        RuleFor(x => x.token).NotEmpty();
            
        RuleFor(x => x.refreshToken).NotEmpty();

    }
}