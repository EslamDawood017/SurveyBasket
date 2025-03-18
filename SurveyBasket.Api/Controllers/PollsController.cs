using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Interfaces;
using SurveyBasket.Api.Models;

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
    public IActionResult GetAll()
    {
        return Ok(_pollService.GetAll());
    }
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var poll = _pollService.Get(id);

        return poll == null ? NotFound() : Ok(poll);
    }
    [HttpPost]
    public IActionResult Add(Poll poll)
    {
        var NewPoll = _pollService.Add(poll);

        return CreatedAtAction(nameof(Get), new { id = NewPoll.Id }, NewPoll);
    }
    [HttpPut("{id}")]
    public IActionResult Update(int id , Poll poll)
    {
        var IsUpdated = _pollService.Update(id, poll);

        if(!IsUpdated)
            return NotFound();

        return NoContent();
    }
    [HttpDelete("{id}")]
    public IActionResult Delete(int id) 
    {
        var Res = _pollService.Delete(id);


        if (!Res)
            return NotFound();

        return NoContent();
    }
}
