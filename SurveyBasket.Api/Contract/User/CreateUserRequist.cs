namespace SurveyBasket.Api.Contract.User;

public record CreateUserRequist(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string Password,
    IList<string> Roles
);

