namespace SurveyBasket.Api.Contract.Roles;

public record RoleDetailResponse(
    int Id,
    string Name,
    bool IsDeleted,
    IEnumerable<string> Permissions
);
