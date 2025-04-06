using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Interfaces;
using SurveyBasket.Api.Services;

namespace SurveyBasket.Api.Controllers;
[Route("api/polls/{pollId}/[controller]")]
[ApiController]
public class ResultController(IResultService resultService) : ControllerBase
{
    private readonly IResultService _resultService = resultService;

    [HttpGet("row-data")]
    public async Task<IActionResult> PollVotes([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        var result = await _resultService.GetPollVotesAsync(pollId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : Problem(statusCode: result.Error.statusCode, title: result.Error.Code, detail: result.Error.Description);
    }
    [HttpGet("votes-Per-Day")]
    public async Task<IActionResult> PollVotesPerDay([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        var result = await _resultService.GetVotesCountPerDayAsync(pollId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : Problem(statusCode: result.Error.statusCode, title: result.Error.Code, detail: result.Error.Description);
    }
    [HttpGet("votes-Per-Question")]
    public async Task<IActionResult> PollVotesPerQuestion([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        var result = await _resultService.GetVotesCountPerQuestionAsync(pollId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : Problem(statusCode: result.Error.statusCode, title: result.Error.Code, detail: result.Error.Description);
    }

}
