namespace SurveyBasket.Api.Contract.Registeration;

public record RegisterationRequist(
    int Id,
    string Email,
    string Password,
    string FirstName,
    string LastName);

