namespace SurveyBasket.Api.Contract.User;

public record ChangePasswordRequist(
    string CurrentPassword,
    string NewPassword
);

