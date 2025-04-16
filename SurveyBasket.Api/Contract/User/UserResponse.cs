namespace SurveyBasket.Api.Contract.User;

public record UserResponse(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    bool IsDisabled,
    IEnumerable<string> Roles
    );

