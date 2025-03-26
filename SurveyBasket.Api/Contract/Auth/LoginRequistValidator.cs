using FluentValidation;
using Microsoft.AspNetCore.Identity.Data;

namespace SurveyBasket.Api.Contract.auth;

public class LoginRequistValidator : AbstractValidator<LoginRequest>
{
    public LoginRequistValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password).NotEmpty();

    }
}