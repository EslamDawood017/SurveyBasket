using FluentValidation;

namespace SurveyBasket.Api.Contract.Vote;

public class VoteRequistValidator : AbstractValidator<VoteRequist>
{
    public VoteRequistValidator()
    {
        RuleFor(x => x.Answers).NotEmpty();

        RuleForEach(x => x.Answers)
            .SetInheritanceValidator(v => v.Add(new VoteAnswerRequistValidator()));
    }
}
