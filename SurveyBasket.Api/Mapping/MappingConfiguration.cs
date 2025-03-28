﻿using Mapster;
using SurveyBasket.Api.Contract.Poll;

namespace SurveyBasket.Api.Mapping;

public class MappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Poll, PollResponse>()
            .Map(dest => dest.Summary, src => src.Summary)
            .TwoWays();
    }
}
