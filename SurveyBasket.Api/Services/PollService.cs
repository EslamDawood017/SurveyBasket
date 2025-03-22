using SurveyBasket.Api.Data;
using SurveyBasket.Api.Entities;
using SurveyBasket.Api.Interfaces;


namespace SurveyBasket.Api.Services;

public class PollService : IPollService
{
    private readonly AppDbContext _context;

    public PollService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Poll>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Polls.AsNoTracking().ToListAsync(cancellationToken);
    }
    public async Task<Poll?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        return  await _context.Polls.FindAsync(id, cancellationToken);
    }

    public async Task<Poll> AddAsync(Poll poll, CancellationToken cancellationToken = default)
    {
        await _context.Polls.AddAsync(poll ,cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return poll;
    }

    public async Task<bool> UpdateAsync(int id, Poll poll, CancellationToken cancellationToken)
    {
        Poll UpdatedPoll = await GetAsync(id , cancellationToken);

        if (UpdatedPoll == null)
            return false;

        UpdatedPoll.Title = poll.Title;
        UpdatedPoll.Summary = poll.Summary;
        UpdatedPoll.EndsAt = poll.EndsAt;
        UpdatedPoll.StartsAt = poll.StartsAt;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(int id , CancellationToken cancellationToken)
    {
        Poll poll = await GetAsync(id);

        if (poll == null)
            return false;

        _context.Polls.Remove(poll);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
    public async Task<bool> TogglePublishStatusAsync(int id, CancellationToken cancellationToken)
    {
        Poll poll = await GetAsync(id, cancellationToken);

        if (poll == null)
            return false;

        poll.IsPublished = !poll.IsPublished;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
