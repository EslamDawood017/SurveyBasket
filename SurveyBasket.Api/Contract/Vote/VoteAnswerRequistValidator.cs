using FluentValidation;

namespace SurveyBasket.Api.Contract.Vote;

public class VoteAnswerRequistValidator : AbstractValidator<VoteAnswerRequist>
{
    public VoteAnswerRequistValidator()
    {
        RuleFor(x => x.QuestionId)
            .GreaterThan(0);

        RuleFor(x => x.AnswerId)
            .GreaterThan(0);

    }
}
