using SurveyBasket.Api.Contract.Auth;

namespace SurveyBasket.Api.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> GetTokenAsync(string email , string password , CancellationToken cancellationToken = default);
}
