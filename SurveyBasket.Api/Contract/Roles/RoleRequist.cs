namespace SurveyBasket.Api.Contract.Roles;

public record RoleRequist(
    string Name , 
    IEnumerable<string> Permission
);

