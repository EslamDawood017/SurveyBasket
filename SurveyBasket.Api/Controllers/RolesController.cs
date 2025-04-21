using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Contract.Roles;
using SurveyBasket.Api.Interfaces;
using SurveyBasket.Authentication.Filters;

namespace SurveyBasket.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class RolesController(IRoleSerivce roleSerivce) : ControllerBase
{
    private readonly IRoleSerivce _roleSerivce = roleSerivce;

    [HttpGet("")]
    [HasPermission(Permissions.GetRole)]
    public async Task<IActionResult> GetAll([FromQuery] bool includeDeleted = false)
    {
        var roles = await _roleSerivce.GetAllAsync(includeDeleted);

        return Ok(roles);
    }
    [HttpGet("{Id}")]
    [HasPermission(Permissions.GetRole)]
    public async Task<IActionResult> GetById(int Id)
    {
        var result = await _roleSerivce.GetById(Id);

        return result.IsSuccess
            ? Ok(result.Value)
            : Problem(title: result.Error.Code, detail: result.Error.Description, statusCode: result.Error.statusCode);
    }
    [HttpPost("")]
    [HasPermission(Permissions.AddRole)]
    public async Task<IActionResult> Add([FromBody] RoleRequist requist)
    {
        var result = await _roleSerivce.AddAsync(requist);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { result.Value.Id }, result.Value)
            : Problem(title: result.Error.Code, detail: result.Error.Description, statusCode: result.Error.statusCode);
    }
    [HttpPut("{id}")]
    [HasPermission(Permissions.UpdateRole)]
    public async Task<IActionResult> Update(int id, [FromBody] RoleRequist requist)
    {
        var result = await _roleSerivce.UpdateAsync(id, requist);

        return result.IsSuccess
            ? NoContent()
            : Problem(title: result.Error.Code, detail: result.Error.Description, statusCode: result.Error.statusCode);
    }
    [HttpPut("toggle/{id}")]
    [HasPermission(Permissions.UpdateRole)]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var result = await _roleSerivce.ToggleStatusAsync(id);

        return result.IsSuccess
            ? NoContent()
            : Problem(title: result.Error.Code, detail: result.Error.Description, statusCode: result.Error.statusCode);
    }

}
