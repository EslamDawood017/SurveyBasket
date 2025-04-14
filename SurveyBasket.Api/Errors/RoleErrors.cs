using SurveyBasket.Api.Abstractions;

namespace SurveyBasket.Api.Errors;

public class RoleErrors
{
    public static readonly Error RoleNotFound = new Error("Role.RoleNotFound", "No Role Found", StatusCodes.Status404NotFound);
    public static readonly Error InvalidPermission = new Error("Role.InvalidPermission", "Invalid Permission", StatusCodes.Status400BadRequest);
    public static readonly Error DuplicatedRole = new Error("Role.DuplicatedRole", "this role is already exist", StatusCodes.Status409Conflict);

}
