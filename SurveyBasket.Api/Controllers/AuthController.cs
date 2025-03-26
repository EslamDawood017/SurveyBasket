using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Contract.Auth;
using SurveyBasket.Api.Interfaces;
using SurveyBasket.Api.Services;

namespace SurveyBasket.Api.Controllers;
[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    [HttpPost("Login")]
    public async Task<IActionResult> Login( LoginRequist loginRequist , CancellationToken cancellationToken)
    {
        var authResult = await _authService.GetTokenAsync(loginRequist.email, loginRequist.password, cancellationToken);

        if (authResult == null)
            return BadRequest("Invalid UserName or Password");

        return Ok(authResult);
    }
}
