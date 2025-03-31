using Microsoft.AspNetCore.Identity;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Authentications;
using SurveyBasket.Api.Contract.Auth;
using SurveyBasket.Api.Errors;
using SurveyBasket.Api.Interfaces;
using System.Security.Cryptography;

namespace SurveyBasket.Api.Services;

public class AuthService(UserManager<ApplicationUser> userManager , IJwtProvider jwtProvider) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly int _refreshTokenExpireDays = 14;
    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        ApplicationUser user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredetials);

        var isValidPassword = await _userManager.CheckPasswordAsync(user, password);
        
        if (!isValidPassword)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredetials);


        var (token , expiredIn) = _jwtProvider.GenerateToken(user);

        var refreshToken = GenerateRefreshToken();

        var refreshTokenExpirtions = DateTime.UtcNow.AddDays(_refreshTokenExpireDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpirtions
        });

        await _userManager.UpdateAsync(user);

        var response = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiredIn, refreshToken, refreshTokenExpirtions);

        return Result.Success<AuthResponse>(response);
    }
    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredetials);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredetials);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredetials);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        var (newToken, expiredIn) = _jwtProvider.GenerateToken(user);

        var NewRefreshToken = GenerateRefreshToken();

        var refreshTokenExpirtions = DateTime.UtcNow.AddDays(_refreshTokenExpireDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = NewRefreshToken,
            ExpiresOn = refreshTokenExpirtions
        });

        await _userManager.UpdateAsync(user);

        var response = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, newToken, expiredIn, NewRefreshToken, refreshTokenExpirtions);
        
        return Result.Success<AuthResponse>(response);

    }
    public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
            return Result.Failure(UserErrors.InvalidCredetials);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure(UserErrors.InvalidCredetials);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
            return Result.Failure(UserErrors.InvalidCredetials);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return Result.Success();
    }
    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
