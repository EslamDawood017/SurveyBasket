namespace SurveyBasket.Api.Authentications;

public interface IJwtProvider
{
    (string Token, int ExpiredIn) GenerateToken(ApplicationUser applicationUser , IEnumerable<string> roles , IEnumerable<string> permissions);
    string? ValidateToken(string token );
}
