﻿using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Authentications;
using SurveyBasket.Api.Contract.Auth;
using SurveyBasket.Api.Contract.Registeration;
using SurveyBasket.Api.Data;
using SurveyBasket.Api.Errors;
using SurveyBasket.Api.Helpers;
using SurveyBasket.Api.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace SurveyBasket.Api.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IEmailSender emailSender,
    IHttpContextAccessor httpContextAccessor,
    IJwtProvider jwtProvider,
    AppDbContext context,
    ILogger<AuthService> logger) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly AppDbContext _context = context;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly int _refreshTokenExpireDays = 14;
    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredetials);

        if (!user.IsActive)
            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

        var result = await _signInManager.PasswordSignInAsync(user, password, false, true);

        if (result.Succeeded)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var permission = await _context.Roles
                .Join(_context.RoleClaims,
                    role => role.Id,
                    claim => claim.RoleId,
                    (role, claim) => new { role, claim })
                .Where(x => roles.Contains(x.role.Name!))
                .Select(x => x.claim.ClaimValue)
                .Distinct()
                .ToListAsync(cancellationToken);

            var (token, expiredIn) = _jwtProvider.GenerateToken(user, roles, permission!);

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

        var error = result.IsNotAllowed
            ? UserErrors.EmailNotComfirmed
            : (result.IsLockedOut ? UserErrors.LockedUser : UserErrors.InvalidCredetials);

        return Result.Failure<AuthResponse>(error);
    }
    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredetials);


        var user = await _userManager.FindByIdAsync(userId);

        if (!user!.IsActive)
            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredetials);

        if (user.LockoutEnd > DateTime.UtcNow)
            return Result.Failure<AuthResponse>(UserErrors.LockedUser);


        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredetials);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        var roles = await _userManager.GetRolesAsync(user);

        var permission = await _context.Roles
            .Join(_context.RoleClaims,
                role => role.Id,
                claim => claim.RoleId,
                (role, claim) => new { role, claim })
            .Where(x => roles.Contains(x.role.Name!))
            .Select(x => x.claim.ClaimValue)
            .Distinct()
            .ToListAsync(cancellationToken);


        var (newToken, expiredIn) = _jwtProvider.GenerateToken(user, roles, permission);

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
    public async Task<Result> RegisterAsync(RegisterationRequist requist, CancellationToken cancellationToken = default)
    {
        var isExist = await _userManager.Users.AnyAsync(x => x.Email == requist.Email, cancellationToken);

        if (isExist)
            return Result.Failure<AuthResponse>(UserErrors.DuplicatedEmail);

        var user = new ApplicationUser
        {
            Id = requist.Id,
            Email = requist.Email,
            UserName = requist.Email,
            FirstName = requist.FirstName,
            LastName = requist.LastName
        };

        var result = await _userManager.CreateAsync(user, requist.Password);

        if (result.Succeeded)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            _logger.LogInformation("Confirmation Code is :  {code}", code);

            await SendConfirmationEmail(user, code);

            return Result.Success();
        }

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
    public async Task<Result> ConfirmEmail(ConfirmEmailRequist requist)
    {
        var user = await _userManager.FindByIdAsync(requist.UserId.ToString());

        if (user is null)
            return Result.Failure(UserErrors.InvalidCode);

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmation);

        var code = requist.Code;

        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            return Result.Failure(UserErrors.InvalidCode);
        }

        var result = await _userManager.ConfirmEmailAsync(user, code);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, DefaultRoles.Member.Name);
            return Result.Success();
        }


        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
    public async Task<Result> ResendConfirmationEmailRequistAsync(ResendConfirmationEmailRequist requist)
    {
        var user = await _userManager.FindByEmailAsync(requist.Email);

        if (user is null)
            return Result.Success();

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmation);


        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));


        await SendConfirmationEmail(user, code);

        _logger.LogInformation("Confirmation Code is :  {code}", code);

        return Result.Success();
    }
    public async Task<Result> SendResetPasswordCodeAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return Result.Success();

        if (!user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailNotComfirmed with { statusCode = StatusCodes.Status400BadRequest });


        var code = await _userManager.GeneratePasswordResetTokenAsync(user);

        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));


        await SendResetPasswordEmail(user, code);

        _logger.LogInformation("Reset Code is :  {code}", code);

        return Result.Success();
    }
    private async Task SendConfirmationEmail(ApplicationUser user, string code)
    {
        var origin = _httpContextAccessor.HttpContext!.Request.Headers.Origin;

        var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation", new Dictionary<string, string>
            {
                { "{{name}}" , user.FirstName },
                { "{{action_url}}" , $"{origin}/auth/emailConfirmation?UserId={user.Id}&code={code}" }
            });

        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Survey Basket : Email Confirmation", emailBody));

        await Task.CompletedTask;
    }
    private async Task SendResetPasswordEmail(ApplicationUser user, string code)
    {
        var origin = _httpContextAccessor.HttpContext.Request.Headers.Origin;

        var emailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword", new Dictionary<string, string>
            {
                { "{{name}}" , user.FirstName },
                { "{{action_url}}" , $"{origin}/auth/emailResetPassword?UserId={user.Email}&code={code}" }
            });

        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Survey Basket :Change Password", emailBody));

        await Task.CompletedTask;
    }
    public async Task<Result> ResetPasswordAsync(ResetPasswordRequist requist)
    {
        var user = await _userManager.FindByEmailAsync(requist.Email);

        if (!user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailNotComfirmed);

        if (user is null)
            return Result.Failure(UserErrors.InvalidCode);

        IdentityResult result;

        try
        {
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(requist.Code));
            result = await _userManager.ResetPasswordAsync(user, code, requist.NewPaswood);
        }
        catch (FormatException)
        {
            result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
        }

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status401Unauthorized));
    }
}

