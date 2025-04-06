using Mapster;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contract.Question;
using SurveyBasket.Api.Contract.Vote;
using SurveyBasket.Api.Data;
using SurveyBasket.Api.Entities;
using SurveyBasket.Api.Errors;
using SurveyBasket.Api.Interfaces;

namespace SurveyBasket.Api.Services;

public class VoteService(AppDbContext context) : IVoteService
{
    private readonly AppDbContext _context = context;

    public async Task<Result> AddAsync(int pollId, int userId, VoteRequist voteRequist, CancellationToken cancellationToken = default)
    {
        var hasVoted = await _context.Votes.AnyAsync(v => v.PollId == pollId && v.ApplicationUserId == userId, cancellationToken);
        if (hasVoted)
            return Result.Failure<ICollection<QuestionResponse>>(VoteError.DuplicatedVote);
        var isPollExist = await _context.Polls.AnyAsync(p => p.Id == pollId && p.IsPublished && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow));

        if (!isPollExist)
            return Result.Failure<ICollection<QuestionResponse>>(PollError.PollNotFound);

        var availableQuestions = await _context.Questions
            .Where(x => x.PollId == pollId && x.IsActive)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if(!voteRequist.Answers.Select(a => a.QuestionId).SequenceEqual(availableQuestions))
            return Result.Failure<ICollection<QuestionResponse>>(VoteError.InvalidQuestions);

        var vote = new Vote
        {
            PollId = pollId,
            ApplicationUserId = userId,
            VoteAnswers = voteRequist.Answers.Adapt<ICollection<VoteAnswer>>().ToList(),
        };

        await _context.Votes.AddAsync(vote , cancellationToken);
        await _context.SaveChangesAsync();

        return Result.Success();
    }
}
