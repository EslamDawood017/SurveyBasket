using FluentValidation;
using SurveyBasket.Api.Contract.Requist;

namespace SurveyBasket.Api.Contract.Poll;

public class LoginRequistValidator : AbstractValidator<CreatePollRequist>
{
    public LoginRequistValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("title is reuired")
            .Length(3, 100);

        RuleFor(x => x.Summary)
            .NotEmpty()
            .Length(3, 1000);

        RuleFor(x => x.StartsAt)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));

        RuleFor(x => x.EndsAt)
            .NotEmpty()
            .GreaterThan(x => x.StartsAt);
    }
}
