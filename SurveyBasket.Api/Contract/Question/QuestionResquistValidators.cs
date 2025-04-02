using FluentValidation;

namespace SurveyBasket.Api.Contract.Question;

public class QuestionResquistValidators : AbstractValidator<QuestionRequist>
{
    public QuestionResquistValidators()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .Length(3,1000);

        RuleFor(x => x.Answers)
            .NotNull();

        RuleFor(x=> x.Answers)
            .Must(x => x.Count > 1 )
            .WithMessage("Question should have at leat 2 answers")
            .When(x => x.Answers != null);

        RuleFor(x => x.Answers) 
            .Must(x => x.Distinct().Count() == x.Count())
            .WithMessage("you can't add duplicated answers for the same question ")
            .When(x => x.Answers != null);
    }
}
