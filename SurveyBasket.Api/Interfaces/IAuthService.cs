using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contract.Auth;
using SurveyBasket.Api.Contract.Registeration;

namespace SurveyBasket.Api.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<Result> RegisterAsync(RegisterationRequist requist, CancellationToken cancellationToken = default);
    Task<Result> ConfirmEmail(ConfirmEmailRequist requist);
    Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    Task<Result> ResendConfirmationEmailRequistAsync(ResendConfirmationEmailRequist requist);
    Task<Result> SendResetPasswordCodeAsync(string email);
    Task<Result> ResetPasswordAsync(ResetPasswordRequist requist);
}
