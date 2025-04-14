using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Contract.Registeration;
using SurveyBasket.Api.Contract.User;
using SurveyBasket.Api.Extentions;
using SurveyBasket.Api.Interfaces;

namespace SurveyBasket.Api.Controllers;
[Route("me")]
[ApiController]
[Authorize]
public class UsersController(IUserService userService ) : ControllerBase
{
    private readonly IUserService _userService = userService;


    [HttpGet("")]
    public async Task<IActionResult> GetUserInfo()
    {
        var result = await _userService.UserProfileAsync(User.GetUserId());

        return Ok(result.Value);
    }
    [HttpPut("info")]
    public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserProfileRequist requist)
    {
        await _userService.UpdateUserInfoAsync(User.GetUserId() , requist);

        return NoContent();
    }
    [HttpPut("Change-Password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequist requist)
    {
        var result = await _userService.ChangePassword(User.GetUserId(), requist);

        return result.IsSuccess
            ? NoContent()
            : Problem(title: result.Error.Code, detail: result.Error.Description, statusCode: StatusCodes.Status400BadRequest);    
    }
    
}
