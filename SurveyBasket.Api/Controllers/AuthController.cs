using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using SurveyBasket.Api.Authentications;
using SurveyBasket.Api.Contract.Auth;
using SurveyBasket.Api.Contract.Registeration;
using SurveyBasket.Api.Interfaces;

namespace SurveyBasket.Api.Controllers;
[Route("[controller]")]
[ApiController]
[EnableRateLimiting("IPLimiter")]
public class AuthController(
    ILogger<AuthController> logger,
    IAuthService authService,
    IOptions<JwtOptions> options,
    IOptionsSnapshot<JwtOptions> optionsSnapshot,
    IOptionsMonitor<JwtOptions> optionsMonitor
    ) : ControllerBase
{
    private readonly ILogger<AuthController> logger = logger;
    private readonly IAuthService _authService = authService;
    private readonly IOptions<JwtOptions> _options = options;
    private readonly IOptionsSnapshot<JwtOptions> _optionsSnapshot = optionsSnapshot;
    private readonly IOptionsMonitor<JwtOptions> _optionsMonitor = optionsMonitor;

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequist loginRequist, CancellationToken cancellationToken)
    {
        var result = await _authService.GetTokenAsync(loginRequist.email, loginRequist.password, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : Problem(statusCode: result.Error.statusCode, title: result.Error.Code, detail: result.Error.Description);
    }
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequist Requist, CancellationToken cancellationToken)
    {
        var result = await _authService.GetRefreshTokenAsync(Requist.token, Requist.refreshToken, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);
    }
    [HttpPost("revoke-refresh-token")]
    public async Task<IActionResult> RevokeResfreshToken(RefreshTokenRequist Requist, CancellationToken cancellationToken)
    {
        var result = await _authService.RevokeRefreshTokenAsync(Requist.token, Requist.refreshToken, cancellationToken);

        return result.IsSuccess ? Ok() : Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);

    }
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterationRequist Requist, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(Requist, cancellationToken);

        return result.IsSuccess ? Ok() : Problem(statusCode: result.Error.statusCode, title: result.Error.Code, detail: result.Error.Description);
    }
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequist Requist, CancellationToken cancellationToken)
    {
        var result = await _authService.ConfirmEmail(Requist);

        return result.IsSuccess ? Ok() : Problem(statusCode: result.Error.statusCode, title: result.Error.Code, detail: result.Error.Description);
    }
    [HttpPost("resend-email-confirmation")]
    public async Task<IActionResult> ResendEmailConfirmation(ResendConfirmationEmailRequist Requist)
    {
        var result = await _authService.ResendConfirmationEmailRequistAsync(Requist);

        return result.IsSuccess ? Ok() : Problem(statusCode: result.Error.statusCode, title: result.Error.Code, detail: result.Error.Description);
    }
    [HttpPut("forget-Password")]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequist requist)
    {
        var result = await _authService.SendResetPasswordCodeAsync(requist.Email);

        return result.IsSuccess
            ? NoContent()
            : Problem(title: result.Error.Code, detail: result.Error.Description, statusCode: StatusCodes.Status400BadRequest);
    }
    [HttpPut("reset-Password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequist requist)
    {
        var result = await _authService.ResetPasswordAsync(requist);

        return result.IsSuccess
            ? NoContent()
            : Problem(title: result.Error.Code, detail: result.Error.Description, statusCode: StatusCodes.Status400BadRequest);
    }
}
