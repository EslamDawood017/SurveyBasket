using SurveyBasket.Api.Abstractions;

namespace SurveyBasket.Api.Errors;

public class UserErrors
{
    public static Error InvalidCredetials = new Error("User.InvalidCredetional", "Invalid Email or password");
}
