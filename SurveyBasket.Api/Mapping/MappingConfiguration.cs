using Mapster;
using SurveyBasket.Api.Contract.Poll;
using SurveyBasket.Api.Contract.Question;

namespace SurveyBasket.Api.Mapping;

public class MappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Poll, PollResponse>()
            .Map(dest => dest.Summary, src => src.Summary)
            .TwoWays();


        config.NewConfig<QuestionRequist, Question>()
            .Map(dest => dest.Answers, src => src.Answers.Select( Answer =>  new Answer { Content = Answer}) );
    }
}
