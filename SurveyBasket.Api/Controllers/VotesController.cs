using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SurveyBasket.Api.Contract.Vote;
using SurveyBasket.Api.Errors;
using SurveyBasket.Api.Extentions;
using SurveyBasket.Api.Interfaces;


namespace SurveyBasket.Api.Controllers;
[Route("api/poll/{pollId}/vote")]
[ApiController]
[Authorize(Roles = DefaultRoles.Member.Name)]
[EnableRateLimiting("Concurrency")]
public class VotesController(IQuestionService questionService, IVoteService voteService) : ControllerBase
{
    private readonly IQuestionService _questionService = questionService;
    private readonly IVoteService _voteService = voteService;
    [HttpGet("")]
    public async Task<IActionResult> Start([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        var UserId = User.GetUserId();

        var result = await _questionService.GetAvailableAsync(pollId, UserId!, cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Value);

        return result.Error.Code == VoteError.DuplicatedVote.Code
            ? Problem(statusCode: StatusCodes.Status400BadRequest, title: result.Error.Code, detail: result.Error.Description)
            : Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);

    }
    [HttpPost("")]
    public async Task<IActionResult> Vote([FromRoute] int pollId, [FromBody] VoteRequist requist, CancellationToken cancellationToken)
    {
        var result = await _voteService.AddAsync(pollId, User.GetUserId(), requist, cancellationToken);

        if (result.IsSuccess)
            return Created();

        return result.Error.Code == PollError.PollNotFound.Code
           ? Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description)
           : Problem(statusCode: StatusCodes.Status400BadRequest, title: result.Error.Code, detail: result.Error.Description);

    }
}

