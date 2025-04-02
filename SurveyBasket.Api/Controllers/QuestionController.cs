using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Contract.Question;
using SurveyBasket.Api.Errors;
using SurveyBasket.Api.Interfaces;
using SurveyBasket.Api.Services;

namespace SurveyBasket.Api.Controllers;
[Route("api/polls/{pollId}/[controller]")]
[ApiController]
[Authorize]
public class QuestionController(IQuestionService questionService) : ControllerBase
{
    private readonly IQuestionService _questionService = questionService;

    [HttpGet("{Id}")]
    public async Task<IActionResult> Get([FromRoute]int pollId ,[FromRoute] int Id , CancellationToken cancellationToken)
    {
        var result = await _questionService.GetAsync(pollId, Id, cancellationToken);

        if(result.IsSuccess) 
            return Ok(result.Value);

        return Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);
    }
    [HttpGet("")]
    public async Task<IActionResult> GetAll([FromRoute] int pollId , CancellationToken cancellationToken)
    {
        var result = await _questionService.GetAllAsync(pollId ,cancellationToken);

        if(result.IsSuccess) 
            return Ok(result.Value);

        return Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromRoute] int pollId , [FromBody]QuestionRequist requist , CancellationToken cancellationToken)
    {
        var result = await _questionService.AddAsync(pollId , requist , cancellationToken);   

        if(result.IsSuccess)
            return CreatedAtAction(nameof(Get) , new {pollId , result.Value.Id} , result.Value);

        return result.Error == QuestionError.DuplicatedQuestionContent
            ? Problem(statusCode: StatusCodes.Status400BadRequest, title: result.Error.Code, detail: result.Error.Description)
            : Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int pollId,[FromRoute] int id , [FromBody] QuestionRequist requist, CancellationToken cancellationToken)
    {
        var result = await _questionService.UpdateAsync(pollId, id , requist, cancellationToken);

        if (result.IsSuccess)
            return NoContent();

        return result.Error == QuestionError.DuplicatedQuestionContent
            ? Problem(statusCode: StatusCodes.Status400BadRequest, title: result.Error.Code, detail: result.Error.Description)
            : Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);
    }
    [HttpPut("{Id}/toggleStatus")]
    public async Task<IActionResult> toggleStatus([FromRoute] int pollId, [FromRoute] int Id, CancellationToken cancellationToken)
    {
        var result = await _questionService.ToggleStatusAsync(pollId, Id, cancellationToken);

        if (result.IsSuccess)
            return Ok();

        return Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);
    }
}
