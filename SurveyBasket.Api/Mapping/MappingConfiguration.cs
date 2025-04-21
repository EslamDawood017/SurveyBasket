using Mapster;
using SurveyBasket.Api.Contract.Poll;
using SurveyBasket.Api.Contract.Question;
using SurveyBasket.Api.Contract.User;

namespace SurveyBasket.Api.Mapping;

public class MappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Poll, PollResponse>()
            .Map(dest => dest.Summary, src => src.Summary)
            .TwoWays();


        config.NewConfig<QuestionRequist, Question>()
            .Map(dest => dest.Answers, src => src.Answers.Select(Answer => new Answer { Content = Answer }));

        config.NewConfig<UpdateUserRequist, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email)
            .Map(dest => dest.NormalizedUserName, src => src.Email.ToUpper());
    }
}
