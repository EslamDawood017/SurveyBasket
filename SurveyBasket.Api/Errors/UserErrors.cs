using SurveyBasket.Api.Abstractions;

namespace SurveyBasket.Api.Errors;

public class UserErrors
{
    public static Error EmailNotComfirmed = new Error("User.EmailNotComfirmed", "email is not confirmed", StatusCodes.Status401Unauthorized);
    public static Error InvalidCredetials = new Error("User.InvalidCredetional", "Invalid Email or password", StatusCodes.Status401Unauthorized);
    public static Error DuplicatedEmail = new Error("User.DuplicatedEmail", "Another user with the same email is already exists", StatusCodes.Status409Conflict);
    public static Error InvalidCode = new Error("User.InvalidCode", "Invalid Code", StatusCodes.Status400BadRequest);
    public static Error DuplicatedConfirmation = new Error("User.DuplicatedConfirmation", "Email is already confirmed", StatusCodes.Status400BadRequest);
}
