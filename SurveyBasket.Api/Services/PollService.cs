using SurveyBasket.Api.Interfaces;
using SurveyBasket.Api.Models;

namespace SurveyBasket.Api.Services;

public class PollService : IPollService
{
    private static readonly List<Poll> _polls = new List<Poll>
    {
        new Poll
        {
            Id = 1,
            Description = "Hello World",
            Title = "Cairo"
        }
    };
    public IEnumerable<Poll> GetAll()
    {
        return _polls;
    }
    public Poll? Get(int id)
    {
        return _polls.FirstOrDefault(p => p.Id == id);
    }

    public Poll Add(Poll poll)
    {
        poll.Id = _polls.Count + 1;
        _polls.Add(poll);
        return poll;
    }

    public bool Update(int id, Poll poll)
    {
        Poll UpdatedPoll = Get(id);

        if(UpdatedPoll ==  null)
            return false;

        UpdatedPoll.Title = poll.Title;
        UpdatedPoll.Description = poll.Description;

        return true;
    }

    public bool Delete(int id)
    {
        Poll poll = Get(id);

        if (poll == null)
            return false;

        _polls.Remove(poll);

        return true;
    }
}
