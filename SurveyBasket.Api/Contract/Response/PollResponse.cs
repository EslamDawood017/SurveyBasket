namespace SurveyBasket.Api.Contract.Response;

public record PollResponse(int Id,
    string Title, 
    string Summary,
    bool IsPublished,
    DateOnly StartsAt,
    DateOnly EndsAt
    );

