﻿using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.RateLimiting;
using SurveyBasket.Api.Consts;
using SurveyBasket.Api.Contract.Poll;
using SurveyBasket.Api.Contract.Requist;
using SurveyBasket.Api.Errors;
using SurveyBasket.Api.Interfaces;
using SurveyBasket.Authentication.Filters;



namespace SurveyBasket.Api.Controllers;
[Route("api/[controller]")]


public class PollsController : ControllerBase
{
    private IPollService _pollService;

    public PollsController(IPollService pollService)
    {
        _pollService = pollService;
    }

    [HttpGet]
    [HasPermission(Permissions.GetPolls)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {      
        var result = await _pollService.GetAllAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);
    }
    [HttpGet("Current")]
    [Authorize(Roles = DefaultRoles.Member)]
    
    public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
    {
        var result = await _pollService.GetCurrentAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);
    }
    [HttpGet("{id}")]
    [HasPermission(Permissions.GetPolls)]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.GetAsync(id, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }
    [HttpPost]
    [HasPermission(Permissions.AddPolls)]
    public async Task<IActionResult> Add([FromBody] PollRequist requist, CancellationToken cancellationToken)
    {

        var result = await _pollService.AddAsync(requist.Adapt<Poll>(), cancellationToken);

        return result.IsSuccess 
            ? CreatedAtAction(nameof(Get), new { id = result.Value.Id }, result.Value)
            : Problem(statusCode: StatusCodes.Status400BadRequest , title : result.Error.Code , detail : result.Error.Description);
    }
    [HttpPut("{id}")]
    [HasPermission(Permissions.UpdatePolls)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequist requist, CancellationToken cancellationToken)
    {
        var result = await _pollService.UpdateAsync(id, requist, cancellationToken);

        if (result.IsSuccess)
            return NoContent();

        return result.Error == PollError.DuplicatedPollTitle
             ? Problem(statusCode: StatusCodes.Status400BadRequest, title: result.Error.Code, detail: result.Error.Description)
             : Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);
    }



    [HttpDelete("{id}")]
    [HasPermission(Permissions.DeletePolls)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var Res = await _pollService.DeleteAsync(id, cancellationToken);

        return Res.IsSuccess ? NoContent() : Problem(statusCode:StatusCodes.Status404NotFound , title : Res.Error.Code , detail : Res.Error.Description);
    }
    [HttpPut("{id}/TogglePublish")]
    [HasPermission(Permissions.GetPolls)]
    public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.TogglePublishStatusAsync(id, cancellationToken);

        return (result.IsSuccess) ? NoContent() : Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);

    }
}
