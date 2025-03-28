using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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

        if (authResult == null)
            return BadRequest("Invalid UserName or Password");

        return Ok(authResult);
    }
    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequist Requist, CancellationToken cancellationToken)
    {
        var authResult = await _authService.GetRefreshTokenAsync(Requist.token, Requist.refreshToken, cancellationToken);

        if (authResult == null)
            return BadRequest("Invalid token");

        return Ok(authResult);
    }
    [HttpPost("revoke-refresh-token")]
    public async Task<IActionResult> RevokeResfreshToken(RefreshTokenRequist Requist, CancellationToken cancellationToken)
    {
        var isRevoked = await _authService.RevokeRefreshTokenAsync(Requist.token, Requist.refreshToken, cancellationToken);

        if (!isRevoked)
            return BadRequest("Operation failed");

        return Ok();
    }

}
