namespace SurveyBasket.Api.Contract.Requist;

public record PollRequist(
    string Title,
    string Summary,
    DateOnly StartsAt,
    DateOnly EndsAt
    );
