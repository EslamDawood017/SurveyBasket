namespace SurveyBasket.Api.Contract.Auth;

public record ResetPasswordRequist(
    string Email,
    string Code,
    string NewPaswood
);
