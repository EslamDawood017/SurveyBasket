using Mapster;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contract.Poll;
using SurveyBasket.Api.Contract.Requist;
using SurveyBasket.Api.Data;
using SurveyBasket.Api.Entities;
using SurveyBasket.Api.Errors;
using SurveyBasket.Api.Interfaces;
using System.Collections.Generic;


namespace SurveyBasket.Api.Services;

public class PollService : IPollService
{
    private readonly AppDbContext _context;

    public PollService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Result<IEnumerable<PollResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var PollsList =  await _context.Polls.AsNoTracking().ToListAsync(cancellationToken);
        
        if (!PollsList.Any())
            return Result.Failure<IEnumerable<PollResponse>>(PollError.EmptyList);

        return Result.Success(PollsList.Adapt<IEnumerable<PollResponse>>());
    }
    public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.FindAsync(id, cancellationToken);

        if(poll is null) 
            return Result.Failure<PollResponse>(PollError.PollNotFound);

        return Result.Success<PollResponse>(poll.Adapt<PollResponse>());
    }

    public async Task<Result<PollResponse>> AddAsync(Poll poll, CancellationToken cancellationToken = default)
    {
        var isTitleExists = await _context.Polls.AnyAsync(p => p.Title == poll.Title );
        
        if (isTitleExists)
            return Result.Failure<PollResponse>(PollError.DuplicatedPollTitle);

        await _context.Polls.AddAsync(poll, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(poll.Adapt<PollResponse>());
    }

    public async Task<Result> UpdateAsync(int id, PollRequist poll, CancellationToken cancellationToken)
    {

        Poll UpdatedPoll = await _context.Polls.FindAsync(id , cancellationToken);

        if (UpdatedPoll == null)
            return Result.Failure(PollError.PollNotFound);

        var isTitleExists = await _context.Polls.AnyAsync(p => p.Title == poll.Title && p.Id != id);

        if (isTitleExists)
            return Result.Failure<PollResponse>(PollError.DuplicatedPollTitle);

        UpdatedPoll.Title = poll.Title;
        UpdatedPoll.Summary = poll.Summary;
        UpdatedPoll.EndsAt = poll.EndsAt;
        UpdatedPoll.StartsAt = poll.StartsAt;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        Poll? poll = await _context.Polls.FindAsync(id, cancellationToken);

        if (poll == null)
            return Result.Failure(PollError.PollNotFound);

        _context.Polls.Remove(poll);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
    public async Task<Result> TogglePublishStatusAsync(int id, CancellationToken cancellationToken)
    {
        Poll? poll = await _context.Polls.FindAsync(id, cancellationToken);

        if (poll == null)
            return Result.Failure(PollError.PollNotFound);

        poll.IsPublished = !poll.IsPublished;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
