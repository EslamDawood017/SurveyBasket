using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contract.Auth;

namespace SurveyBasket.Api.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponse>> GetTokenAsync(string email , string password , CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);

}
