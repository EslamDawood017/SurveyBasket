using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Authentications;
using SurveyBasket.Api.Contract.Auth;
using SurveyBasket.Api.Interfaces;
using SurveyBasket.Api.Services;

namespace SurveyBasket.Api.Controllers;
[Route("[controller]")]
[ApiController]
public class AuthController(
    IAuthService authService ,
    IOptions<JwtOptions> options ,
    IOptionsSnapshot<JwtOptions> optionsSnapshot ,
    IOptionsMonitor<JwtOptions> optionsMonitor
    ) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly IOptions<JwtOptions> _options = options;
    private readonly IOptionsSnapshot<JwtOptions> _optionsSnapshot = optionsSnapshot;
    private readonly IOptionsMonitor<JwtOptions> _optionsMonitor = optionsMonitor;

    [HttpPost("Login")]
    public async Task<IActionResult> Login( LoginRequist loginRequist , CancellationToken cancellationToken)
    {
        var authResult = await _authService.GetTokenAsync(loginRequist.email, loginRequist.password, cancellationToken);

        if (authResult!.IsFailure)
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
               
                title: "Bad Request" );
        return Ok(authResult.Value);
    }
    [HttpPost("Refresh")]
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
}
