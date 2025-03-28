namespace SurveyBasket.Api.Contract.Requist;

public record CreatePollRequist(string Title,
    string Summary,
    DateOnly StartsAt,
    DateOnly EndsAt
    );
