namespace SurveyBasket.Api.Contract.Auth;

public record RefreshTokenRequist(
    string token,
    string refreshToken
    );

