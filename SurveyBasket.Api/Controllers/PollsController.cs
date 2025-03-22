using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SurveyBasket.Api.Contract.Requist;
using SurveyBasket.Api.Contract.Response;
using SurveyBasket.Api.Interfaces;



namespace SurveyBasket.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PollsController : ControllerBase
{
    private IPollService _pollService;

    public PollsController(IPollService pollService)
    {
        _pollService = pollService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll( CancellationToken cancellationToken)
    {
        var Polls = await _pollService.GetAllAsync(cancellationToken);

        var respose = Polls.Adapt<IEnumerable<PollResponse>>();

        return Ok(respose);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        var poll = await _pollService.GetAsync(id, cancellationToken);

        return poll == null ? NotFound() : Ok(poll.Adapt<PollResponse>());
    }
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreatePollRequist requist , CancellationToken cancellationToken)
    {

        var NewPoll = await _pollService.AddAsync(requist.Adapt<Poll>() , cancellationToken);

        return CreatedAtAction(nameof(Get), new { id = NewPoll.Id }, NewPoll);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CreatePollRequist poll , CancellationToken cancellationToken )
    {
        var IsUpdated = await _pollService.UpdateAsync(id, poll.Adapt<Poll>() , cancellationToken);

        if (!IsUpdated)
            return NotFound();

        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id , CancellationToken cancellationToken)
    {
        var Res = await _pollService.DeleteAsync(id , cancellationToken);


        if (!Res)
            return NotFound();

        return NoContent();
    }
    [HttpPut("{id}/TogglePublish")]
    public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken)
    {
        var IsUpdated = await _pollService.TogglePublishStatusAsync(id, cancellationToken);

        if (!IsUpdated)
            return NotFound();

        return NoContent();
    }
}
