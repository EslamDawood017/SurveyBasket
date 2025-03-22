namespace SurveyBasket.Api.Contract.Requist;

public record CreatePollRequist(string Title,
    string Summary,
    bool IsPublished,
    DateOnly StartsAt,
    DateOnly EndsAt
    );
