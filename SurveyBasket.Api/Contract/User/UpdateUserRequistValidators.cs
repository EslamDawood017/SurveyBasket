﻿using FluentValidation;

namespace SurveyBasket.Api.Contract.User;

public class UpdateUserRequistValidators : AbstractValidator<UpdateUserRequist>
{
    public UpdateUserRequistValidators()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Roles)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.Roles)
            .Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("You can't Duplicate role for the same user")
            .When(x => x.Roles != null);


    }
}
