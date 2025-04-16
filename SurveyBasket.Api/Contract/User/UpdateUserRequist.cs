namespace SurveyBasket.Api.Contract.User;

public record UpdateUserRequist(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    IList<string> Roles
);

