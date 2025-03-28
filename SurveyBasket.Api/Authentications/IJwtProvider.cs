namespace SurveyBasket.Api.Authentications;

public interface IJwtProvider
{
    (string Token, int ExpiredIn) GenerateToken(ApplicationUser applicationUser );
    string? ValidateToken(string token );
}
