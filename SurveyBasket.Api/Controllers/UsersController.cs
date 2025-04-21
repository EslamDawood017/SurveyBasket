using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Contract.User;
using SurveyBasket.Api.Extentions;
using SurveyBasket.Api.Interfaces;
using SurveyBasket.Authentication.Filters;

namespace SurveyBasket.Api.Controllers;
[Route("me")]
[ApiController]

public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet("all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllAsync(cancellationToken);

        return Ok(users);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
        var result = await _userService.GetAsync(id);

        return result.IsSuccess
            ? Ok(result.Value)
            : Problem(title: result.Error.Code, detail: result.Error.Description, statusCode: StatusCodes.Status400BadRequest);

    }
    [Authorize]
    [HttpGet("")]
    public async Task<IActionResult> GetUserInfo()
    {
        var result = await _userService.UserProfileAsync(User.GetUserId());

        return Ok(result.Value);
    }
    [HttpPut("info")]
    [Authorize]
    public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserProfileRequist requist)
    {
        await _userService.UpdateUserInfoAsync(User.GetUserId(), requist);

        return NoContent();
    }
    [HttpPut("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequist requist)
    {
        var result = await _userService.ChangePassword(User.GetUserId(), requist);

        return result.IsSuccess
            ? NoContent()
            : Problem(title: result.Error.Code, detail: result.Error.Description, statusCode: StatusCodes.Status400BadRequest);
    }
    [HttpPost("")]
    [HasPermission(Permissions.AddUser)]
    public async Task<IActionResult> Add([FromBody] CreateUserRequist requist, CancellationToken cancellationToken)
    {
        var result = await _userService.AddAsync(requist, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { result.Value.Id }, result.Value)
            : Problem(title: result.Error.Code, detail: result.Error.Description, statusCode: StatusCodes.Status400BadRequest);

    }
    [HttpPut("")]
    [HasPermission(Permissions.UpdateUser)]
    public async Task<IActionResult> Update([FromBody] UpdateUserRequist requist, CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateAsync(requist, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : Problem(title: result.Error.Code, detail: result.Error.Description, statusCode: StatusCodes.Status400BadRequest);

    }
    [HttpPut("toggle-user/{id}")]
    [HasPermission(Permissions.UpdateUser)]
    public async Task<IActionResult> Update([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _userService.ToggleStatusAsync(id, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : Problem(title: result.Error.Code, detail: result.Error.Description, statusCode: StatusCodes.Status400BadRequest);
    }
    [HttpPut("unlock/{id}")]
    [HasPermission(Permissions.UpdateUser)]
    public async Task<IActionResult> Unlock([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _userService.Unlock(id, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : Problem(title: result.Error.Code, detail: result.Error.Description, statusCode: StatusCodes.Status400BadRequest);
    }

}
